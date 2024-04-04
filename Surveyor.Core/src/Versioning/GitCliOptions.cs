namespace Surveyor.Versioning;

/// <summary>
/// Options for <see cref="GitCli"/>.
/// </summary>
public class GitCliOptions
{
    /// <summary>
    /// The section key for binding options.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0#bind-hierarchical-configuration-data-using-the-options-pattern"/>
    public const string Section = "Git";

    /// <summary>
    /// The root directory of the git repository.
    /// </summary>
    public string Directory { get; set; } = string.Empty;

    /// <summary>
    /// Should validation of the root directory be skipped?
    /// </summary>
    public bool SkipValidation { get; set; } = false;
}
