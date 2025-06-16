using System.Diagnostics;
using Shoko.Plugin.Abstractions.Enums;

namespace AnizipProvider.model;

public class AnizipFile
{
    public int Id { get; set; }
    public int AnimeId { get; set; }
    public int EpisodeId { get; set; }
    public long FileSize { get; set; }
    public string? MD5 { get; set; }
    public string? CRC32 { get; set; }
    public string ED2K { get; set; }
    public string Extension { get; set; }
    public long ReleasedAt { get; set; }
    public string QualityType { get; set; }
    public AnizipSourceType SourceType { get; set; }
    public string? Notes { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
    public int UserCount { get; set; }
    public string? SHA1 { get; set; }
    public long Length { get; set; }
    public string Type { get; set; }
    public bool AvdumpVerified { get; set; }
    public int Version { get; set; }
    public bool? CrcMatches { get; set; }
    public bool? Censored { get; set; }
    public bool HasChapters { get; set; }
    public AnizipGroup? Group { get; set; }
    public List<AnizipFileEpisodeRelation> Relations { get; set; }

    /// <summary>
    /// Gets the ReleaseSource for the file
    /// </summary>
    /// <returns>The mapped ReleaseSource</returns>
    public ReleaseSource ShokoSource()
    {
        return SourceType switch
        {
            AnizipSourceType.Tv or AnizipSourceType.Dtv or AnizipSourceType.Hdtv => ReleaseSource.TV,
            AnizipSourceType.Dvd or AnizipSourceType.Hkdvd or AnizipSourceType.HdDvd => ReleaseSource.DVD,
            AnizipSourceType.Www => ReleaseSource.Web,
            AnizipSourceType.Vhs => ReleaseSource.VHS,
            AnizipSourceType.Vcd or AnizipSourceType.Svcd => ReleaseSource.VCD,
            AnizipSourceType.Ld => ReleaseSource.LaserDisc,
            AnizipSourceType.Camcorder => ReleaseSource.Camera,
            AnizipSourceType.BluRay => ReleaseSource.BluRay,
            _ => ReleaseSource.Unknown
        };
    }
}