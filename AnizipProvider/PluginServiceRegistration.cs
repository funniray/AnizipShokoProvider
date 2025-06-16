using Microsoft.Extensions.DependencyInjection;
using Shoko.Plugin.Abstractions;

namespace AnizipProvider;

/// <inheritdoc/>
public class PluginServiceRegistration : IPluginServiceRegistration
{
    /// <inheritdoc/>
    public void RegisterServices(IServiceCollection serviceCollection, IApplicationPaths applicationPaths)
    {
        serviceCollection.AddSingleton<AnizipClient>();
    }
}