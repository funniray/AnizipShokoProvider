namespace AnizipProvider.model;

/// <summary>
/// Represents an anime release group.
/// </summary>
public class AnizipGroup
{
    /// <summary>
    /// The unique identifier of the group.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The name of the group.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a tag associated with the group.
    /// </summary>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// The website URL of the group.
    /// </summary>
    public string Website { get; set; } = string.Empty;

    /// <summary>
    /// The email address of the group.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The IRC server used by the group.
    /// </summary>
    public string IrcServer { get; set; } = string.Empty;

    /// <summary>
    /// The IRC channel used by the group.
    /// </summary>
    public string IrcChannel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a description of the group.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The creation timestamp (Unix epoch).
    /// </summary>
    public long CreatedAt { get; set; }

    /// <summary>
    /// The last update timestamp (Unix epoch).
    /// </summary>
    public long UpdatedAt { get; set; }

    /// <summary>
    /// The group's rating score.
    /// </summary>
    public float Rating { get; set; }

    /// <summary>
    /// The number of votes for the group.
    /// </summary>
    public int? Votes { get; set; }
}