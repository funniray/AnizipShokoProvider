using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Shoko.Plugin.Abstractions.Config;
using Shoko.Plugin.Abstractions.Config.Attributes;
using Shoko.Plugin.Abstractions.Config.Enums;

namespace AnizipProvider.model;

public class AnizipConfiguration : IReleaseInfoProviderConfiguration
{
    /// <summary>
    /// Anizip API Base URL
    /// </summary>
    [Badge("Debug", Theme = DisplayColorTheme.Warning)]
    [Visibility(Advanced = true, Size = DisplayElementSize.Full)]
    [Url]
    [Required]
    [DefaultValue("https://test.ani.zip")]
    public string Host { get; set; } = "https://test.ani.zip";
}