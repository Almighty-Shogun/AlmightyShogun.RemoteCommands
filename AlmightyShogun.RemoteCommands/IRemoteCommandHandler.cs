namespace AlmightyShogun.RemoteCommands;

public interface IRemoteCommandHandler
{
    /// <summary>
    /// Starts the asynchronous listener for handling remote commands.
    /// </summary>
    /// 
    /// <returns>A task that represents the asynchronous operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public Task StartAsync();
}