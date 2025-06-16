namespace AnizipProvider.model;

public class AnizipGroup
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public string Website { get; set; }
    public string Email { get; set; }
    public string IrcServer { get; set; }
    public string IrcChannel { get; set; }
    public string Description { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
    public float Rating { get; set; }
    public int? Votes { get; set; }
}