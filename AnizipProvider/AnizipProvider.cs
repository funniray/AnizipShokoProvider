using System.Diagnostics;
using System.Reflection;
using AnizipProvider.model;
using Microsoft.Extensions.Logging;
using Shoko.Plugin.Abstractions.DataModels;
using Shoko.Plugin.Abstractions.Enums;
using Shoko.Plugin.Abstractions.Hashing;
using Shoko.Plugin.Abstractions.Release;
using Shoko.Plugin.Abstractions.Services;

namespace AnizipProvider;

public class AnizipProvider(AnizipClient anizipClient, ILogger<AnizipProvider> logger, IAnidbService aniDbService) : IReleaseInfoProvider<AnizipConfiguration>
{
    /// <inheritdoc/>
    public async Task<ReleaseInfo?> GetReleaseInfoForVideo(ReleaseInfoRequest request, CancellationToken cancellationToken)
    {
        var video = request.Video;
        var timer = new Stopwatch();
        timer.Start();
        var file = await anizipClient.GetAnizipFileByED2K(video.ED2K);
        var time = timer.ElapsedMilliseconds;
        
        logger.LogInformation($"Looked up ED2K {video.ED2K} in {time}ms");
        
        var info = ConvertFile(file);

        if (info is not null)
        {
            foreach (var animeId in info.CrossReferences.Select(xref => xref.AnidbAnimeID).Distinct())
            {
                if (animeId is null) {continue;}
                await aniDbService.RefreshByID(animeId.Value, AnidbRefreshMethod.Default, cancellationToken: cancellationToken);
            }
        }

        return info;
    }

    private static DateTime ConvertDate(long seconds)
    {
        // Should this be UTC or Local? Does it matter?
        return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
    }

    private ReleaseInfo? ConvertFile(AnizipFile? file)
    {
        if (file is null) { return null; }

        List<ReleaseVideoCrossReference> xref = [new() { AnidbAnimeID = file.AnimeId, AnidbEpisodeID = file.EpisodeId }];
        
        ReleaseGroup? group = null;
        
        if (file.Group is not null)
        {
            group = new()
            {
                Name = file.Group.Name,
                ShortName = file.Group.Tag,
                ID = file.Group.Id.ToString(),
                Source = "AniDB"
            };
        }

        foreach (var relation in file.Relations)
        {
            xref.Add(new()
            {
                AnidbAnimeID = relation.AnimeId, 
                AnidbEpisodeID = relation.EpisodeId, 
                PercentageEnd = relation.EndPercentage, 
                PercentageStart = relation.StartPercentage
            });
        }

        List<HashDigest> hashes = [];

        if (file.CRC32 is not null)
        {
            hashes.Add(new() {Type = "CRC32", Value = file.CRC32});
        }

        if (file.SHA1 is not null)
        {
            hashes.Add(new () {Type = "SHA1", Value = file.SHA1});
        }

        if (file.MD5 is not null)
        {
            hashes.Add(new () {Type = "MD5", Value = file.MD5});
        }

        return new ReleaseInfo
        {
            ID = file.Id.ToString(),
            Version = file.Version,
            Comment = file.Notes,
            CrossReferences = xref,
            FileSize = file.FileSize,
            Group = group,
            IsChaptered = file.HasChapters,
            IsCorrupted = file.QualityType == "CORRUPTED",
            IsCensored = file.Censored,
            ReleasedAt = DateOnly.FromDateTime(ConvertDate(file.ReleasedAt)),
            Hashes = hashes,
            ReleaseURI = $"https://anidb.net/file/{file.Id}",
            Source = file.ShokoSource()
        };
    }

    /// <inheritdoc/>
    public async Task<ReleaseInfo?> GetReleaseInfoById(string releaseId, CancellationToken cancellationToken)
    {
        var timer = new Stopwatch();
        timer.Start();
        var file = await anizipClient.GetAnizipFileById(releaseId);
        var time = timer.ElapsedMilliseconds;
        
        logger.LogInformation($"Looked up FileID {releaseId} in {time}ms");

        var info = ConvertFile(file);

        if (info is not null)
        {
            foreach (var animeId in info.CrossReferences.Select(xref => xref.AnidbAnimeID).Distinct())
            {
                if (animeId is null) {continue;}
                await aniDbService.RefreshByID(animeId.Value, AnidbRefreshMethod.Default, cancellationToken: cancellationToken);
            }
        }

        return info;
    }

    /// <inheritdoc/>
    public string Name { get; } = "Anizip Provider";
    /// <inheritdoc/>
    public Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version!;
    /// <inheritdoc/>
    public string Description { get; } = """
                                             Provides an alternative method to resolving AniDB metadata for files.
                                         """;
}