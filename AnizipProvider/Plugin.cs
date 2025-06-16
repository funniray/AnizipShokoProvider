using Shoko.Plugin.Abstractions;

namespace AnizipProvider;

/// <inheritdoc/>
public class Plugin: IPlugin
{
    /// <inheritdoc/>
    public string Name { get; } = "Anizip Provider";
    /// <inheritdoc/>
    public string Description { get; } = "Provides an alternative method to resolving AniDB metadata for files";
}