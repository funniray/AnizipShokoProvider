using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Enums;

namespace AnizipProvider.model;

/// <summary>
/// Configuration for the Anizip release provider.
/// </summary>
public class AnizipConfiguration : IReleaseInfoProviderConfiguration, IPluginConfiguration
{
    /// <summary>
    /// The Anizip API base URL.
    /// </summary>
    [Badge("Debug", Theme = DisplayColorTheme.Warning)]
    [Visibility(Advanced = true, Size = DisplayElementSize.Full)]
    [Url]
    [Required]
    [DefaultValue("https://files.ani.zip")]
    public string Host { get; set; } = "https://files.ani.zip";
}