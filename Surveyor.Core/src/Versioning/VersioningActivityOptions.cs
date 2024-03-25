namespace Surveyor.Versioning;

/// <summary>
/// Options for <see cref="VersioningActivity"/>.
/// </summary>
public class VersioningActivityOptions
{
    /// <summary>
    /// The section key for binding options.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0#bind-hierarchical-configuration-data-using-the-options-pattern"/>
    public const string Section = "Versioning";

    /// <summary>
    /// The name of the branch.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// The name of the package.
    /// </summary>
    public string PackageName { get; set; } = string.Empty;

    /// <summary>
    /// The directory containing the source files for the project.
    /// </summary>
    public string ProjectDirectory { get; set; } = string.Empty;
}
