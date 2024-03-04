using System.Text.Json;
using Microsoft.Extensions.Options;
using Surveyor.Utils.Versioning;

namespace Surveyor.Packages;

/// <summary>
/// A wrapper for querying the NuGet packages API.
/// </summary>
/// <remarks>
/// The NuGet API is rather fickle as servers differ in their implementations.
/// This implementation intends to provide a seamless experience by working with only the required resources.
/// </remarks>
/// <seealso href="https://learn.microsoft.com/en-us/nuget/api/overview"/>
/// <seealso href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource"/>
public class PackageApi
{
    private readonly HttpClient _http;
    private JsonElement[]? _resources;

    /// <summary>
    /// Creates a new instance of <see cref="PackageApi"/>.
    /// </summary>
    public PackageApi(PackageApiOptions options)
    {
        _http = new()
        {
            BaseAddress = new(options.Feed),
            DefaultRequestHeaders =
            {
                Accept = { new("application/json") },
                Authorization = new(options.AuthToken)
            }
        };
    }

    /// <summary>
    /// DI constructor for <see cref="PackageApi"/>.
    /// </summary>
    public PackageApi(IOptions<PackageApiOptions> options) : this(options.Value)
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
        JsonElement? metadataQuery = await GetPackageMetadata(packageName);
        if (metadataQuery is not JsonElement metadata)
            return Array.Empty<SemanticVersion>();
        JsonElement[] items = metadata.GetProperty("items")
            .EnumerateArray()
            .ToArray();
        return items
            .Select(x => x.GetProperty("catalogEntry"))
            .Where(x => includePrerelease || !x.GetProperty("isPrerelease").GetBoolean())
            .Select(x => x.GetProperty("version").GetString() ?? throw new("Failed to get version."))
            .Select(x => SemanticVersion.Create(x) ?? throw new("Failed to parse version."))
            .OrderByDescending(x => x)
            .ToArray();
    }

    /// <summary>
    /// Get the metadata for a package.
    /// </summary>
    /// <param name="packageName">The name of the package.</param>
    /// <returns>The package metadata as a <see cref="JsonElement"/>.</returns>
    private async Task<JsonElement?> GetPackageMetadata(string packageName)
    {
        string baseUri = await GetResourceUri("RegistrationsBaseUrl");
        string uri = $"{baseUri}/{packageName.ToLower()}/index.json";
        HttpResponseMessage response = await _http.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
            return null;
        string content = await response.Content.ReadAsStringAsync();
        JsonDocument json = JsonDocument.Parse(content);
        JsonElement[] items = json.RootElement.GetProperty("items").EnumerateArray().ToArray();
        if (items.Length == 0)
            return null;
        // TODO: Warning if more than one item

        return items.FirstOrDefault();
    }

    /// <summary>
    /// Get the resource URI for a given resource type.
    /// </summary>
    private async Task<string> GetResourceUri(string type)
    {
        JsonElement[] resources = await GetResources();
        JsonElement resource = resources
            .FirstOrDefault(resource =>
            {
                JsonElement json = resource.GetProperty("@type");
                return json.GetString() == type;
            });
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
        HttpResponseMessage response = await _http.GetAsync("index.json");
        if (!response.IsSuccessStatusCode)
            throw new("Failed to get index.json.");
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
