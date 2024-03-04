namespace Surveyor.Packages;

/// <summary>
/// Options for <see cref="PackageApi"/>.
/// </summary>
public class PackageApiOptions
{
    private const string DefaultFeed = "https://api.nuget.org/v3/index.json";

    /// <summary>
    /// The section name for <see cref="PackageApiOptions"/>.
    /// </summary>
    public const string Section = "Packages";

    /// <summary>
    /// The URL of the NuGet feed.
    /// </summary>
    public string Feed { get; set; } = DefaultFeed;

    /// <summary>
    /// An optional authentication token for the NuGet feed.
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;
}
