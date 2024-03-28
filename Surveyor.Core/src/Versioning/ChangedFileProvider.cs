using Microsoft.Extensions.Logging;

namespace Surveyor.Versioning;

/// <summary>
/// A strategy to provide a collection of changed file names.
/// </summary>
public interface IChangedFileProvider
{

    /// <summary>
    /// Get a collection of file names that have had changes since the beginning.
    /// </summary>
    /// <param name="projectDirectory">The path of the project directory.</param>
    /// <returns>
    /// A collection of versions.
    /// </returns>
    public IEnumerable<string> Get(string projectDirectory);

    /// <summary>
    /// Get a collection of file names that have had changes since <paramref name="version"/>.
    /// </summary>
    /// <param name="projectDirectory">The path of the project directory.</param>
    /// <param name="version">The since version.</param>
    /// <returns>
    /// A collection of versions.
    /// </returns>
    public IEnumerable<string> Get(string projectDirectory, SemanticVersion version);
}

/// <inheritdoc/>
public class ChangedFileProvider : IChangedFileProvider
{
    private readonly ILogger<ChangedFileProvider> _logger;
    private readonly GitCli _git;

    /// <summary>
    /// DI constructor for <see cref="ChangedFileProvider"/>.
    /// </summary>
    public ChangedFileProvider(ILogger<ChangedFileProvider> logger, GitCli git)
    {
        _logger = logger;
        _git = git;
    }

    /// <inheritdoc/>
    public IEnumerable<string> Get(string projectDirectory)
    {
        return GetInternal(projectDirectory, null);
    }

    /// <inheritdoc/>
    public IEnumerable<string> Get(string projectDirectory, SemanticVersion version)
    {
        string sinceRef = $"v{version}";
        return GetInternal(projectDirectory, sinceRef);
    }

    private IEnumerable<string> GetInternal(string projectDirectory, string? sinceRef)
    {
        string absoluteProjectDirectory = Path.GetFullPath(projectDirectory)
            .Replace('\\', '/');
        if (absoluteProjectDirectory.StartsWith(_git.RootDirectory) is false)
        {
            _logger.LogError("The project directory is outside the root directory.");
            return Enumerable.Empty<string>();
        }
        string relativeProjectDirectory = absoluteProjectDirectory.Substring(_git.RootDirectory.Length + 1);
        IReadOnlyCollection<string> changedFiles = string.IsNullOrEmpty(sinceRef)
            ? _git.GetAllFiles()
            : _git.GetFilesChangedSince(sinceRef!);
        return changedFiles
            .Where(path => path.StartsWith(relativeProjectDirectory));
    }
}
