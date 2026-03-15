namespace AnizipProvider.model;

/// <summary>
/// Represents the relationship between a file and an episode.
/// </summary>
public class AnizipFileEpisodeRelation
{
    /// <summary>
    /// The unique identifier of the relation.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The associated file ID.
    /// </summary>
    public long FileId { get; set; }

    /// <summary>
    /// The associated anime ID.
    /// </summary>
    public int AnimeId { get; set; }

    /// <summary>
    /// The associated episode ID.
    /// </summary>
    public int EpisodeId { get; set; }

    /// <summary>
    /// The start percentage of the episode in this file.
    /// </summary>
    public int StartPercentage { get; set; }

    /// <summary>
    /// The end percentage of the episode in this file.
    /// </summary>
    public int EndPercentage { get; set; }

    /// <summary>
    /// The creation timestamp.
    /// </summary>
    public int CreatedAt { get; set; }
}