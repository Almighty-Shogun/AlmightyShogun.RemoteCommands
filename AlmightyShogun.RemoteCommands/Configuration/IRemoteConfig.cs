using System.Net;

namespace AlmightyShogun.RemoteCommands.Configuration;

public interface IRemoteConfig
{
    public string Address { get; set; }
    public int Port { get; set; }
    public HashSet<string> WhitelistedIpAddresses { get; }
}
