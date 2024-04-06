namespace Surveyor.ReleaseNotes;

/// <summary>
/// Options for <see cref="ReleaseNotesActivity"/>.
/// </summary>
public class ReleaseNotesActivityOptions
{
    /// <summary>
    /// The section key for binding options.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0#bind-hierarchical-configuration-data-using-the-options-pattern"/>
    public const string Section = "Releases";

    /// <summary>
    /// The name of the branch.
    /// </summary>
    public string Branch { get; set; } = string.Empty;
}
