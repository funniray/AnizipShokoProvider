using System.Diagnostics;
using System.Reflection;
using AnizipProvider.model;
using Microsoft.Extensions.Logging;
using Shoko.Abstractions.Extensions;
using Shoko.Abstractions.Metadata.Anidb.Enums;
using Shoko.Abstractions.Metadata.Anidb.Services;
using Shoko.Abstractions.Video.Hashing;
using Shoko.Abstractions.Video.Release;

namespace AnizipProvider;

/// <summary>
/// AniZip Provider.
/// </summary>
/// <param name="anizipClient">API Client</param>
/// <param name="logger">Logger</param>
/// <param name="aniDbService">AniDB Service</param>
public class AnizipProvider(AnizipClient anizipClient, ILogger<AnizipProvider> logger, IAnidbService aniDbService) : IReleaseInfoProvider<AnizipConfiguration>
{
    /// <inheritdoc/>
    public async Task<ReleaseInfo?> GetReleaseInfoForVideo(ReleaseInfoContext context, CancellationToken cancellationToken)
    {
        var (video, _) = context;
        var timer = new Stopwatch();
        timer.Start();
        AnizipFile? file;
        try
        {
            file = await anizipClient.GetAnizipFileByED2K(video.ED2K);
        } 
        catch (HttpRequestException ex)
        {
            logger.LogError($"Failed to lookup hash ${video.ED2K} with status code ${ex.StatusCode}.\n${ex.Message}");
            throw ex;
        }

        var time = timer.ElapsedMilliseconds;

        logger.LogInformation($"Looked up ED2K {video.ED2K} in {time}ms");

        var info = ConvertFile(file);

        if (info is not null)
        {
            foreach (var xref in info.CrossReferences)
            {
                var animeId = xref.AnidbAnimeID;
                if (animeId is not null)
                {
                    await aniDbService.RefreshAnimeByID(animeId.Value, AnidbRefreshMethod.Default | AnidbRefreshMethod.SkipSupplementaryUpdate, cancellationToken: cancellationToken);
                }
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

        List<ReleaseVideoCrossReference> xref = 
        [
            ReleaseVideoCrossReference.ForAniDB(file.EpisodeId, file.AnimeId)
        ];

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
            xref.Add(ReleaseVideoCrossReference.ForAniDB(
                episodeID: relation.EpisodeId,
                animeID: relation.AnimeId,
                percentStart: relation.StartPercentage,
                percentEnd: relation.EndPercentage
            ));
        }

        List<HashDigest> hashes = [new() { Type = "ED2K", Value = file.ED2K }];

        if (file.CRC32 is { Length: > 0 })
        {
            hashes.Add(new() {Type = "CRC32", Value = file.CRC32});
        }

        if (file.SHA1 is { Length: > 0 })
        {
            hashes.Add(new () {Type = "SHA1", Value = file.SHA1});
        }

        if (file.MD5 is { Length: > 0 })
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
