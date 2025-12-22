using Microsoft.Extensions.DependencyInjection;
using AlmightyShogun.RemoteCommands.Configuration;

namespace AlmightyShogun.RemoteCommands;

public static class RemoteCommandsLoader
{
    /// <summary>
    /// Registers remote command services with the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// 
    /// <param name="serviceCollection">The service collection used to register dependencies related to console command functionality.</param>
    /// <param name="config">An optional action used to configure the remote commands connection handler.</param>
    ///
    /// <returns>The <see cref="IServiceCollection"/> instance with the remote commands handlers registered.</returns>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static IServiceCollection AddRemoteCommands(this IServiceCollection serviceCollection, Action<IRemoteConfig>? config = null)
    {
        if (config is not null)
        {
            RemoteConfig remoteConfig = new();
            
            config(remoteConfig);

            serviceCollection.AddSingleton<IRemoteConfig>(remoteConfig);
        }
        else
        {
            serviceCollection.AddSingleton<IRemoteConfig, RemoteConfig>();
        }
        
        return serviceCollection.AddSingleton<IRemoteCommandHandler, RemoteCommandHandler>();
    }
}
