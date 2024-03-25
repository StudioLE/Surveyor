using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Surveyor.System;
using Surveyor.Versioning;

namespace Surveyor.Packages;

/// <summary>
/// A wrapper for querying the NuGet packages API.
/// </summary>
/// <remarks>
/// The NuGet API is rather fickle as servers differ in their implementations.
/// This implementation intends to provide a seamless experience by working with only the required resources.
/// This is a low level wrapper working directly with the JSON API without using the
/// <see href="https://www.nuget.org/packages/NuGet.Protocol">Nuget.Protocol</see> package.
/// </remarks>
/// <seealso href="https://learn.microsoft.com/en-us/nuget/api/overview"/>
/// <seealso href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource"/>
public class PackageApi
{
    private const string DefaultFeed = "https://api.nuget.org/v3/index.json";
    private readonly ILogger<PackageApi> _logger;
    private readonly HttpClient _http;
    private JsonElement[]? _resources;

    /// <summary>
    /// Creates a new instance of <see cref="PackageApi"/>.
    /// </summary>
    public PackageApi(ILogger<PackageApi> logger, PackageApiOptions options)
    {
        _logger = logger;
        Uri feed = string.IsNullOrEmpty(options.Feed)
            ? new(DefaultFeed)
            : new(options.Feed);
        AuthenticationHeaderValue? auth = string.IsNullOrEmpty(options.AuthToken)
            ? null
            : new(options.AuthToken);
        _http = new()
        {
            BaseAddress = feed,
            DefaultRequestHeaders =
            {
                Accept = { new("application/json") },
                Authorization = auth
            }
        };
    }

    /// <summary>
    /// DI constructor for <see cref="PackageApi"/>.
    /// </summary>
    public PackageApi(ILogger<PackageApi> logger, IOptions<PackageApiOptions> options) : this(logger, options.Value)
    {
    }

    /// <summary>
    /// Get the metadata for a package.
    /// </summary>
    /// <param name="packageName">The name of the package.</param>
    /// <param name="includePrerelease">Should pre release versions be included?</param>
    /// <returns>The package metadata as a <see cref="JsonElement"/>.</returns>
    public async Task<IReadOnlyCollection<SemanticVersion>> GetPackageVersions(string packageName, bool includePrerelease)
    {
        JsonElement[] items = await GetPackageMetadata(packageName);
        return items
            .Select(x => x.GetProperty("catalogEntry"))
            .Select(x => x.GetProperty("version").GetString() ?? throw new("Failed to get version."))
            .Select(x => SemanticVersion.Create(x) ?? throw new("Failed to parse version."))
            .Where(x => includePrerelease || !x.IsPreRelease())
            .OrderByDescending(x => x)
            .ToArray();
    }

    /// <summary>
    /// Get the metadata for a package.
    /// </summary>
    /// <param name="packageName">The name of the package.</param>
    /// <returns>The package metadata as a <see cref="JsonElement"/>.</returns>
    private async Task<JsonElement[]> GetPackageMetadata(string packageName)
    {
        string? baseUri = await GetResourceUri("RegistrationsBaseUrl");
        if (baseUri is null)
            return Array.Empty<JsonElement>();
        baseUri = baseUri.TrimEnd('/');
        string uri = $"{baseUri}/{packageName.ToLower()}/index.json";
        HttpResponseMessage response = await _http.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
            return Array.Empty<JsonElement>();
        string content = await response.Content.ReadAsStringAsync();
        JsonDocument json = JsonDocument.Parse(content);
        JsonElement[] items = json
            .RootElement
            .GetProperty("items")
            .EnumerateArray()
            .ToArray();
        JsonElement[] subItems = items
            .SelectMany(x => x.GetProperty("items").EnumerateArray())
            .ToArray();
        return subItems;
    }

    /// <summary>
    /// Get the resource URI for a given resource type.
    /// </summary>
    private async Task<string?> GetResourceUri(string type)
    {
        JsonElement[] resources = await GetResources();
        if (resources.Length == 0)
            return null;
        JsonElement? resourceQuery = resources
            .FirstOrNull(x =>
            {
                JsonElement json = x.GetProperty("@type");
                return json.GetString() == type;
            });
        if (resourceQuery is not JsonElement resource)
        {
            _logger.LogError("Failed to get resource URI for type: {Type}", type);
            return null;
        }
        string id = resource
                        .GetProperty("@id")
                        .GetString()
                    ?? throw new("Resource ID is missing.");
        return id;
    }

    /// <summary>
    /// Get the resources array the index.json.
    /// </summary>
    private async Task<JsonElement[]> GetResources()
    {
        if (_resources is JsonElement[] resources)
            return resources;
        HttpResponseMessage response;
        try
        {
            response = await _http.GetAsync("index.json");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get resources from NuGet feed: {Feed}", _http.BaseAddress);
            return Array.Empty<JsonElement>();
        }
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get resources from NuGet feed: {Feed}. Response was: {StatusCode}", _http.BaseAddress, response.StatusCode);
            return Array.Empty<JsonElement>();
        }
        string content = await response.Content.ReadAsStringAsync();
        JsonDocument json = JsonDocument.Parse(content);
        resources = json
            .RootElement
            .GetProperty("resources")
            .EnumerateArray()
            .ToArray();
        _resources = resources;
        return resources;
    }
}
