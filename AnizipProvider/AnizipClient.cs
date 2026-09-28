using System.Net;
using System.Reflection;
using AnizipProvider.model;
using Microsoft.Extensions.Logging;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Metadata;
using Shoko.Abstractions.Metadata.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AnizipProvider;

/// <summary>
/// Client that looks up anidb metadata.
/// Uses a semi-private API.
/// </summary>
public class AnizipClient
{
    private readonly string _version = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
    private readonly HttpClient _httpClient;
    private ConfigurationProvider<AnizipConfiguration> _configurationProvider;
    private readonly ILogger<AnizipClient> _logger;
    private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings{Converters = [new StringEnumConverter()]};

    /// <summary>
    /// The source named when ani.zip cannot be reached. Not registered, as ani.zip is not a metadata source of its own.
    /// </summary>
    private static readonly MetadataSource _source = MetadataSource.Parse("ani-zip");

    /// <summary>
    /// Default constructor
    /// </summary>
    public AnizipClient(ConfigurationProvider<AnizipConfiguration> configurationProvider, ILogger<AnizipClient> logger)
    {
        _configurationProvider = configurationProvider;
        _logger = logger;

        _httpClient = new(new SocketsHttpHandler()
        {
            AutomaticDecompression = DecompressionMethods.All,
        })
        {
            DefaultRequestVersion = HttpVersion.Version20,
        };
        _httpClient.DefaultRequestHeaders.Add("User-Agent", $"AnizipShokoProvider ({_version})");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "deflate, zstd;q=1.0, gzip;q=0.9, br;q=0.8, *;q=0.3");
    }

    private AnizipConfiguration GetConfig()
    {
        return _configurationProvider.Load();
    }

    private void LogRequest(HttpResponseMessage response)
    {
        _logger.LogInformation($"Response used http {response.Version}; Cache {response.Headers.GetValues("x-proxy-cache").First()}");
    }

    /// <summary>
    /// Sends a GET request, reporting a failed connection or a timeout as the source being unavailable.
    /// </summary>
    /// <param name="url">The URL to get</param>
    /// <returns>The response</returns>
    /// <exception cref="MetadataProviderUnavailableException">ani.zip could not be reached or did not answer in time.</exception>
    private async Task<HttpResponseMessage> Send(string url)
    {
        try
        {
            return await _httpClient.GetAsync(url);
        }
        catch (HttpRequestException ex)
        {
            throw new MetadataProviderUnavailableException(_source, $"ani.zip could not be reached: {ex.Message}", innerException: ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            throw new MetadataProviderUnavailableException(_source, $"ani.zip did not answer in time: {ex.Message}", innerException: ex);
        }
    }

    /// <summary>
    /// Throws for an unsuccessful response, reporting a server error as the source being unavailable.
    /// </summary>
    /// <param name="response">The response</param>
    /// <exception cref="MetadataProviderUnavailableException">ani.zip answered with a server error.</exception>
    /// <exception cref="HttpRequestException">ani.zip refused the request.</exception>
    private static void EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if ((int)response.StatusCode >= 500)
        {
            throw new MetadataProviderUnavailableException(_source, $"ani.zip answered with {(int)response.StatusCode} {response.ReasonPhrase}.", response.Headers.RetryAfter?.Delta);
        }
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Looks up a file by its ed2k hash
    /// </summary>
    /// <param name="hash">The File's ED2K hash</param>
    /// <returns>File metadata</returns>
    public async Task<AnizipFile?> GetAnizipFileByED2K(string hash)
    {
        var response = await Send($"{GetConfig().Host}/file/ed2k/{hash}");
        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            _logger.LogInformation($"Lookup for ED2K {hash} was not found");
            return null;
        }
        EnsureSuccessStatusCode(response);
        var jsonData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<AnizipFile>(jsonData);
    }

    /// <summary>
    /// Looks up a file by its ID
    /// </summary>
    /// <param name="id">File's AniDB ID</param>
    /// <returns>File Metadata</returns>
    public async Task<AnizipFile?> GetAnizipFileById(string id)
    {
        var response = await Send($"{GetConfig().Host}/file/{id}");
        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        EnsureSuccessStatusCode(response);
        var jsonData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<AnizipFile>(jsonData, _serializerSettings);
    }
}