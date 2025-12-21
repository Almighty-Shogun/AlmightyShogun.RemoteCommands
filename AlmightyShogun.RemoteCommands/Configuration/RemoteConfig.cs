using System.Net;

namespace AlmightyShogun.RemoteCommands.Configuration;

internal sealed class RemoteConfig : IRemoteConfig
{
    public IPAddress Address { get; set; } = IPAddress.Loopback;
    public int Port { get; set; } = 30001;

    public HashSet<IPAddress> WhitelistedIpAddresses { get; set; } =
    [
        IPAddress.Loopback
    ];
}
