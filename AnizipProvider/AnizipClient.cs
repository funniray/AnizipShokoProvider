using System.Net;
using System.Reflection;
using AnizipProvider.model;
using Microsoft.Extensions.Logging;
using Shoko.Plugin.Abstractions.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
    /// Default constructor
    /// </summary>
    public AnizipClient(ConfigurationProvider<AnizipConfiguration> configurationProvider, ILogger<AnizipClient> logger)
    {
        _configurationProvider = configurationProvider;
        _logger = logger;
        
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestVersion = HttpVersion.Version20;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", $"AnizipShokoProvider ({_version})");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
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
    /// Looks up a file by its ed2k hash
    /// </summary>
    /// <param name="hash">The File's ED2K hash</param>
    /// <returns>File metadata</returns>
    public async Task<AnizipFile?> GetAnizipFileByED2K(string hash)
    {
        var response = await _httpClient.GetAsync($"{GetConfig().Host}/file/ed2k/{hash}");
        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
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
        var response = await _httpClient.GetAsync($"{GetConfig().Host}/file/{id}");
        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
        var jsonData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<AnizipFile>(jsonData, _serializerSettings);
    }
}