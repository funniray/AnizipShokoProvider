namespace AnizipProvider.model;

public class AnizipFileEpisodeRelation
{
    public long Id { get; set; }
    public long FileId { get; set; }
    public int AnimeId { get; set; }
    public int EpisodeId { get; set; }
    public int StartPercentage { get; set; }
    public int EndPercentage { get; set; }
    public int CreatedAt { get; set; }
}