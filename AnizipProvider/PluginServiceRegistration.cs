using Microsoft.Extensions.DependencyInjection;
using Shoko.Abstractions.Plugin;

namespace AnizipProvider;

/// <inheritdoc/>
public class PluginServiceRegistration : IPluginServiceRegistration
{
    /// <inheritdoc/>
    public static void RegisterServices(IServiceCollection serviceCollection, IApplicationPaths applicationPaths)
    {
        serviceCollection.AddSingleton<AnizipClient>();
    }
}