namespace Surveyor.Versioning;

/// <summary>
/// Options for <see cref="ProjectVersioningActivity"/>.
/// </summary>
public class VersioningActivityOptions
{
    /// <summary>
    /// The section key for binding options.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0#bind-hierarchical-configuration-data-using-the-options-pattern"/>
    public const string Section = "Versioning";

    /// <summary>
    /// The optional branch name.
    /// Defaults to the current branch.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// The package name.
    /// Required for <see cref="ProjectVersioningActivity"/>.
    /// Not required for <see cref="RepositoryVersioningActivity"/>.
    /// </summary>
    public string Package { get; set; } = string.Empty;

    /// <summary>
    /// The directory containing the source files for the project.
    /// </summary>
    public string Directory { get; set; } = string.Empty;
}
