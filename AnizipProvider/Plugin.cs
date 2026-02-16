using Shoko.Abstractions.Plugin;
using Shoko.Abstractions.Utilities;

namespace AnizipProvider;

/// <inheritdoc/>
public class Plugin: IPlugin
{
    /// <inheritdoc/>
    public Guid ID => UuidUtility.GetV5(typeof(Plugin).FullName!);
    /// <inheritdoc/>
    public string Name { get; } = "Anizip Provider";
    /// <inheritdoc/>
    public string Description { get; } = "Provides an alternative method to resolving AniDB metadata for files";
}