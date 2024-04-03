using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Surveyor.Utils.CommandLine;

namespace Surveyor.Versioning;

/// <summary>
/// Methods to help with Git operations.
/// </summary>
public class GitCli
{
    private readonly Cli _cli = new();

    /// <summary>
    /// The root directory of the git repository.
    /// </summary>
    public string RootDirectory { get; }

    /// <summary>
    /// DI constructor for <see cref="GitCli"/>.
    /// </summary>
    public GitCli(IOptions<GitCliOptions> options) : this(options.Value.Directory)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="GitCli"/>.
    /// </summary>
    /// <remarks>
    /// <paramref name="directory"/> will be validated to ensure it's part of a git repository.
    /// </remarks>
    private GitCli(string directory)
    {
        if(string.IsNullOrEmpty(directory))
            directory = Directory.GetCurrentDirectory();
        RootDirectory = GetRootDirectory(directory);
    }

    private static string GetRootDirectory(string path)
    {
        string absolutePath = Path.GetFullPath(path)
            .Replace('\\', '/')
            .TrimEnd('/');
        if (!ValidatePathArgument(absolutePath))
            throw new("Invalid path argument.");
        if (!Directory.Exists(absolutePath))
            throw new DirectoryNotFoundException("The directory does not exist.");
        IReadOnlyCollection<string> lines = Cli.ExecuteOrThrow("git", $"-C \"{path}\" rev-parse --show-toplevel");
        if (lines.Count != 1)
            throw new($"Failed to determine if the directory is a git repository. Expected 1 line, received {lines.Count}.");
        string rootDirectory = lines.First();
        if (!absolutePath.StartsWith(rootDirectory))
            throw new($"Failed to determine the root directory of: {path}");
        return rootDirectory;
    }

    /// <summary>
    /// Get all the tags in the repository.
    /// </summary>
    /// <returns>
    /// A collection of tags.
    /// </returns>
    public IReadOnlyCollection<string> GetTags()
    {
        return CliExecuteOrThrow("tag --list --sort=-committerdate");
    }

    /// <summary>
    /// Get all the tags on the given branch.
    /// </summary>
    /// <returns>
    /// A collection of tags.
    /// </returns>
    public IReadOnlyCollection<string> GetTagsOnBranch(string branch)
    {
        if (!ValidateGitReferenceArgument(branch))
            throw new("Invalid branch name.");
        return CliExecuteOrThrow($"tag --list --sort=-committerdate --merged \"{branch}\"");
    }

    /// <summary>
    /// Get all the tags pointing at <paramref name="gitReference"/>.
    /// </summary>
    /// <returns>
    /// A collection of tags.
    /// </returns>
    public IReadOnlyCollection<string> GetTagsPointingAt(string gitReference)
    {
        if (!ValidateGitReferenceArgument(gitReference))
            throw new("Invalid branch name.");
        return CliExecuteOrThrow($"tag --points-at \"{gitReference}\"");
    }

    /// <summary>
    /// Get the name of the current branch.
    /// </summary>
    /// <returns>
    /// The name of the current branch.
    /// </returns>
    public string GetCurrentBranch()
    {
        IReadOnlyCollection<string> lines = CliExecuteOrThrow("branch --show-current");
        if (lines.Count != 1)
            throw new($"Failed to determine the current branch. Expected 1 line, received {lines.Count}.");
        return lines.First();
    }

    /// <summary>
    /// Get all the <c>.csproj</c> which have had changes since the git reference <paramref name="sinceRef"/>.
    /// </summary>
    /// <param name="sinceRef">The git reference to search since.</param>
    /// <returns>
    /// A collection of paths to <c>.csproj</c> files relative to the root of the repository.
    /// </returns>
    /// <exception cref="Exception">Thrown if the project file inexplicably doesn't have a directory.</exception>
    public IReadOnlyCollection<string> GetProjectsWithChangesSince(string sinceRef)
    {
        if (!ValidateGitReferenceArgument(sinceRef))
            throw new("Invalid git reference.");
        IReadOnlyCollection<string> changedDirectories = GetDirectoriesWithChangesSince(sinceRef);
        IReadOnlyCollection<string> projects = GetProjectFilePaths();
        return projects
            .Where(project =>
            {
                string relativeDirectory = Path.GetDirectoryName(project) ?? throw new("Failed to get directory");
                // Ignore the case because Windows is tedious
                return changedDirectories.Any(x => x.StartsWith(relativeDirectory, StringComparison.InvariantCultureIgnoreCase));
            })
            .ToArray();
    }

    /// <summary>
    /// Get all the directories containing files that have changed since the given reference.
    /// </summary>
    /// <param name="sinceRef">The git reference to search since.</param>
    /// <returns>
    /// A collection of directory paths with changes relative to the root of the repository.
    /// </returns>
    private IReadOnlyCollection<string> GetDirectoriesWithChangesSince(string sinceRef)
    {
        if (!ValidateGitReferenceArgument(sinceRef))
            throw new("Invalid git reference.");
        IReadOnlyCollection<string> changedFiles = GetFilesChangedSince(sinceRef);
        return changedFiles
            .Select(Path.GetDirectoryName)
            .Distinct()
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();
    }

    /// <summary>
    /// Get all the files in the repository.
    /// </summary>
    /// <returns>
    /// A collection of file paths relative to the root of the repository.
    /// </returns>
    public IReadOnlyCollection<string> GetAllFiles()
    {
        return CliExecuteOrThrow("ls-files");
    }

    /// <summary>
    /// Get all the files that have changed since the given reference.
    /// </summary>
    /// <param name="sinceRef">The git reference to search since.</param>
    /// <returns>
    /// A collection of changed file paths relative to the root of the repository.
    /// </returns>
    public IReadOnlyCollection<string> GetFilesChangedSince(string sinceRef)
    {
        if (!ValidateGitReferenceArgument(sinceRef))
            throw new("Invalid git reference.");
        return CliExecuteOrThrow($"--no-pager diff --name-only \"{sinceRef}\"");
    }

    /// <summary>
    /// Get all the commit messages since the beginning.
    /// </summary>
    /// <returns>
    /// The commit message.
    /// </returns>
    public IReadOnlyCollection<ConventionalCommit> GetAllConventionalCommits()
    {
        IReadOnlyCollection<string> commitReferences = GetAllCommitReferences();
        return CreateConventionalCommits(commitReferences);
    }

    /// <summary>
    /// Get the commit message for the given commit reference.
    /// </summary>
    /// <param name="sinceRef">The git reference to search since.</param>
    /// <returns>
    /// The commit message.
    /// </returns>
    public IReadOnlyCollection<ConventionalCommit> GetConventionalCommitsSince(string sinceRef)
    {
        if (!ValidateGitReferenceArgument(sinceRef))
            throw new("Invalid git reference.");
        IReadOnlyCollection<string> commitReferences = GetCommitReferencesSince(sinceRef);
        return CreateConventionalCommits(commitReferences);
    }

    private ConventionalCommit[] CreateConventionalCommits(IReadOnlyCollection<string> commitReferences)
    {
        ConventionalCommitParser parser = new(new());
        return commitReferences
            .Select(hash =>
            {
                string message = GetCommitMessage(hash);
                ConventionalCommit? commitQuery = parser.Parse(message);
                if (commitQuery is not ConventionalCommit commit)
                    return commitQuery;
                return commit with { Hash = hash };
            })
            .OfType<ConventionalCommit>()
            .ToArray();
    }

    /// <summary>
    /// Get the commit message of the given commit reference.
    /// </summary>
    /// <param name="commitRef">The reference of the commit to retrieve.</param>
    /// <returns>
    /// The commit message.
    /// </returns>
    private string GetCommitMessage(string commitRef)
    {
        if (!ValidateGitReferenceArgument(commitRef))
            throw new("Invalid git reference.");
        IReadOnlyCollection<string> result = CliExecuteOrThrow($"show \"{commitRef}\" --no-patch --format=%B");
        return string.Join(Environment.NewLine, result);
    }

    /// <summary>
    /// Get all the commit hashes.
    /// </summary>
    /// <returns>
    /// The commit hashes.
    /// </returns>
    private IReadOnlyCollection<string> GetAllCommitReferences()
    {
        return CliExecuteOrThrow("rev-list --all");
    }

    /// <summary>
    /// Get all the commit hashes since a given reference.
    /// </summary>
    /// <returns>
    /// The commit hashes.
    /// </returns>
    private IReadOnlyCollection<string> GetCommitReferencesSince(string sinceRef)
    {
        if (!ValidateGitReferenceArgument(sinceRef))
            throw new("Invalid git reference.");
        return CliExecuteOrThrow($"rev-list \"{sinceRef}\"..");
    }

    private IReadOnlyCollection<string> GetProjectFilePaths()
    {
        return EnumerateFilesRecursive(RootDirectory, "*.csproj")
            .Select(path =>
            {
                if (path.StartsWith(RootDirectory))
                    return path.Substring(RootDirectory.Length + 1);
                throw new("Expected an absolute path.");
            })
            .ToArray();
    }

    private static IEnumerable<string> EnumerateFilesRecursive(string directoryPath, string searchPattern)
    {
        IEnumerable<string> matchesInDir = Directory.EnumerateFiles(directoryPath, searchPattern);
        IEnumerable<string> matchesInSubDirs = Directory
            .EnumerateDirectories(directoryPath)
            .SelectMany(x => EnumerateFilesRecursive(x, searchPattern));
        return matchesInDir.Concat(matchesInSubDirs);
    }

    private IReadOnlyCollection<string> CliExecuteOrThrow(string arguments)
    {
        List<string> output = new();
        List<string> errors = new();
        _cli.OnOutput = line => output.Add(line);
        _cli.OnError = line => errors.Add(line);
        _cli.Execute("git", $"-C \"{RootDirectory}\" {arguments}");
        if (errors.Count > 0)
            throw new($"Failed to execute git command: {string.Join(Environment.NewLine, errors)}");
        return output;
    }

    /// <remarks>
    /// The primary purpose is to ensure the <c>"</c> or new line characters are not included which will break the command.
    /// This is a very simply check that ignores many valid references such as tags with /.
    /// </remarks>
    private static bool ValidateGitReferenceArgument(string reference)
    {
        Regex regex = new(@"^[a-z0-9][a-z0-9._\-\/]*$", RegexOptions.IgnoreCase);
        return regex.IsMatch(reference);
    }

    /// <summary>
    /// Validate a path to ensure it is safe to use in a command.
    /// </summary>
    /// <remarks>
    /// The primary purpose is to ensure the <c>"</c> or new line characters are not included which will break the command.
    /// This is a very simply check that likely ignores many valid paths and allows many invalid paths.
    /// </remarks>
    private static bool ValidatePathArgument(string path)
    {
        Regex regex = new(@"^[a-z0-9._\- \/\\#:]+$", RegexOptions.IgnoreCase);
        return regex.IsMatch(path);
    }
}
