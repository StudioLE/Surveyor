using Surveyor.ReleaseNotes;
using Surveyor.VersionControl;

namespace Surveyor.Projects;

/// <summary>
/// Create a formatted index of all <c>.csproj</c> names and descriptions.
/// </summary>
public class ReadMeActivity
{
    private readonly GitCli _git;

    /// <summary>
    /// DI constructor for <see cref="ReleaseNotesActivity"/>.
    /// </summary>
    public ReadMeActivity(GitCli git)
    {
        _git = git;
    }

    /// <inheritdoc cref="ReadMeActivity"/>
    /// <returns>The formatted index.</returns>
    public string Execute()
    {
        IReadOnlyCollection<Project> projectPaths = _git
            .GetAllProjectFilePaths()
            .Where(project => !project.EndsWith("Tests.csproj"))
            .Select(relativePath =>
            {
                string absolutePath = $"{_git.RootDirectory}/{relativePath}";
                return new Project(relativePath, absolutePath);
            })
            .OrderBy(x => x.GetPackageName())
            .ToArray();
        return string.Join("\n\n", projectPaths.Select(CreateReadMeSection));
    }

    private static string CreateReadMeSection(Project project)
    {
        return $"""
                ### [{project.GetPackageName()}]({project.SourceDirectory})

                {project.GetDescription()}
                """;
    }
}
