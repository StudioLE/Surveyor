namespace Surveyor.Packages;

/// <summary>
/// Options for <see cref="PackageApi"/>.
/// </summary>
public class PackageApiOptions
{
    private const string DefaultFeed = "https://api.nuget.org/v3/index.json";

    /// <summary>
    /// The section key for binding options.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0#bind-hierarchical-configuration-data-using-the-options-pattern"/>
    public const string Section = "Packages";

    /// <summary>
    /// The URL of the NuGet feed.
    /// </summary>
    public string Feed { get; set; } = DefaultFeed;

    /// <summary>
    /// An optional authentication token for the NuGet feed.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
