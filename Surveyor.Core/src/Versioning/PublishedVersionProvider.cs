using Surveyor.Packages;

namespace Surveyor.Versioning;

/// <summary>
/// A strategy to provide a collection of versions that have been published.
/// </summary>
public interface IPublishedVersionProvider
{
    /// <summary>
    /// Get a collection of versions that have been published.
    /// </summary>
    /// <param name="packageName">The name of the package.</param>
    /// <returns>
    /// A collection of versions.
    /// </returns>
    public Task<IReadOnlyCollection<SemanticVersion>> Get(string packageName);
}

/// <inheritdoc/>
public class PublishedVersionProvider : IPublishedVersionProvider
{
    private readonly PackageApi _api;

    /// <summary>
    /// DI constructor for <see cref="PublishedVersionProvider"/>.
    /// </summary>
    public PublishedVersionProvider(PackageApi api)
    {
        _api = api;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<SemanticVersion>> Get(string packageName)
    {
        return await _api.GetPackageVersions(packageName, true);
    }
}
