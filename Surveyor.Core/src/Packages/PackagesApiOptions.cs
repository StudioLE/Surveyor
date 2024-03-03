namespace Surveyor.Packages;

/// <summary>
/// Options for <see cref="PackagesApi"/>.
/// </summary>
public class PackagesApiOptions
{
    /// <summary>
    /// The section name for <see cref="PackagesApiOptions"/>.
    /// </summary>
    public const string PackagesSection = "Packages";

    /// <summary>
    /// The NuGet feed base address.
    /// </summary>
    public string BaseAddress { get; set; } = string.Empty;

    /// <summary>
    /// An authentication token for the NuGet feed.
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;
}
