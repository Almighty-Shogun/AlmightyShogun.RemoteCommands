using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using AlmightyShogun.RemoteCommands.Configuration;

namespace AlmightyShogun.RemoteCommands;

public class RemoteCommandHandler : IRemoteCommandHandler
{
    private TcpListener? _listener;
    
    private readonly IRemoteConfig _config;
    private readonly ILogger<RemoteCommandHandler> _logger;
    private readonly Dictionary<string, IRemoteCommand> _commands;

    public RemoteCommandHandler(IRemoteConfig remoteConfig, ILogger<RemoteCommandHandler> logger, IEnumerable<IRemoteCommand> commands)
    {
        _logger = logger;
        _config = remoteConfig;
        _commands = commands.ToDictionary(command => command.Name);
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _listener = new TcpListener(IPAddress.Parse(_config.Address), _config.Port);
        
        _listener.Start();

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Started listening for remote command on {Address:c}:{Port:c}", _config.Address, _config.Port);
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient client = await _listener.AcceptTcpClientAsync(cancellationToken);
            
            await HandleClientAsync(client);
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        if (_listener is null) return;
        
        _listener.Stop();
        _listener = null;
        
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Stopped listening for remote command.");
        }
    }
    
    /// <summary>
    /// Handles communication with a connected client by reading and processing remote commands.
    /// </summary>
    /// 
    /// <param name="client">The connected <see cref="TcpClient"/> representing the remote client.</param>
    /// 
    /// <returns>A task that represents the asynchronous handling of the client's communication.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private async Task HandleClientAsync(TcpClient client)
    {
        var remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
        IPAddress? remoteIp = remoteEndPoint?.Address;

        if (remoteIp == null || !_config.WhitelistedIpAddresses.Contains(remoteIp.ToString()))
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning("Rejected connection from non-whitelisted address {Address:c}", remoteEndPoint);
            }
            
            client.Close();
            
            return;
        }
        
        await using NetworkStream stream = client.GetStream();
        var buffer = new byte[4096];
        
        int bytesRead = await stream.ReadAsync(buffer);
        string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        var payload = JsonSerializer.Deserialize<RemoteCommandPayload>(json);
        
        if (payload == null)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning("Received invalid payload from {Address:c}", client.Client.RemoteEndPoint);
            }
            
            return;
        }
        
        if (_commands.TryGetValue(payload.Command, out IRemoteCommand? handler))
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Received remote command {Command:c} from {Address:c}", payload.Command, client.Client.RemoteEndPoint);
            }
            
            await handler.HandleRawAsync(payload.Data, stream);
        }
        else
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning("Received unknown remote command {Command:c} from {Address:c}", payload.Command, client.Client.RemoteEndPoint);
            }
        }

        client.Close();
    }
}
