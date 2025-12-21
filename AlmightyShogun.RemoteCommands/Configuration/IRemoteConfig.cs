using System.Net;

namespace AlmightyShogun.RemoteCommands.Configuration;

public interface IRemoteConfig
{
    public IPAddress Address { get; set; }
    public int Port { get; set; }
    public HashSet<IPAddress> WhitelistedIpAddresses { get; }
}