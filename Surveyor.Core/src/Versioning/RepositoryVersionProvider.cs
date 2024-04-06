using Surveyor.VersionControl;

namespace Surveyor.Versioning;

/// <summary>
/// A strategy to provide a collection of versions that have been tagged in the repository.
/// </summary>
public interface IRepositoryVersionProvider
{
    /// <summary>
    /// Get a collection of versions that have been tagged in the repository.
    /// </summary>
    /// <returns>
    /// A collection of versions.
    /// </returns>
    public IReadOnlyCollection<SemanticVersion> Get();
}

/// <inheritdoc/>
public class RepositoryVersionProvider : IRepositoryVersionProvider
{
    private readonly GitCli _git;

    /// <summary>
    /// DI constructor for <see cref="RepositoryVersionProvider"/>.
    /// </summary>
    public RepositoryVersionProvider(GitCli git)
    {
        _git = git;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<SemanticVersion> Get()
    {
        IReadOnlyCollection<string> tags = _git.GetTags();
        return tags
            .Where(x => x.StartsWith("v"))
            .Select(SemanticVersion.Create)
            .OfType<SemanticVersion>()
            .ToArray();
    }
}
