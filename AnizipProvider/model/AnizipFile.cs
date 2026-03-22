using Shoko.Abstractions.Video.Enums;

namespace AnizipProvider.model;

/// <summary>
/// Represents an anime file with metadata.
/// </summary>
public class AnizipFile
{
    /// <summary>
    /// The unique identifier of the file.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The associated anime ID.
    /// </summary>
    public int AnimeId { get; set; }

    /// <summary>
    /// The associated episode ID.
    /// </summary>
    public int EpisodeId { get; set; }

    /// <summary>
    /// The file size in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// The MD5 hash of the file.
    /// </summary>
    public string? MD5 { get; set; }

    /// <summary>
    /// The CRC32 checksum of the file.
    /// </summary>
    public string? CRC32 { get; set; }

    /// <summary>
    /// The ED2K hash of the file.
    /// </summary>
    public string ED2K { get; set; } = string.Empty;

    /// <summary>
    /// The file extension.
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// The release timestamp (Unix epoch).
    /// </summary>
    public long ReleasedAt { get; set; }

    /// <summary>
    /// The quality type of the file.
    /// </summary>
    public string QualityType { get; set; } = string.Empty;

    /// <summary>
    /// The source type of the file.
    /// </summary>
    public AnizipSourceType SourceType { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the file.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// The creation timestamp (Unix epoch).
    /// </summary>
    public long CreatedAt { get; set; }

    /// <summary>
    /// The last update timestamp (Unix epoch).
    /// </summary>
    public long UpdatedAt { get; set; }

    /// <summary>
    /// The number of users who have this file.
    /// </summary>
    public int UserCount { get; set; }

    /// <summary>
    /// The SHA1 hash of the file.
    /// </summary>
    public string? SHA1 { get; set; }

    /// <summary>
    /// The length of the file.
    /// </summary>
    public long Length { get; set; }

    /// <summary>
    /// The type of the file.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the file has been verified by Avdump.
    /// </summary>
    public bool AvdumpVerified { get; set; }

    /// <summary>
    /// The version number of the file.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets whether the CRC matches.
    /// </summary>
    public bool? CrcMatches { get; set; }

    /// <summary>
    /// Gets or sets whether the file is censored.
    /// </summary>
    public bool? Censored { get; set; }

    /// <summary>
    /// Gets or sets whether the file has chapters.
    /// </summary>
    public bool HasChapters { get; set; }

    /// <summary>
    /// The associated release group.
    /// </summary>
    public AnizipGroup? Group { get; set; }

    /// <summary>
    /// The episode relations associated with this file.
    /// </summary>
    public List<AnizipFileEpisodeRelation> Relations { get; set; } = [];

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