using Surveyor.VersionControl;

namespace Surveyor.Versioning;

/// <summary>
/// A strategy to provide a collection of versions that have been tagged in the given branch.
/// </summary>
public interface IBranchVersionProvider
{
    /// <summary>
    /// Get a collection of versions that have been tagged in the given branch.
    /// </summary>
    /// <param name="branchName">The name of the branch.</param>
    /// <returns>
    /// A collection of versions.
    /// </returns>
    public IReadOnlyCollection<SemanticVersion> Get(string branchName);
}


/// <inheritdoc />
public class BranchVersionProvider : IBranchVersionProvider
{
    private readonly GitCli _git;

    /// <summary>
    /// DI constructor for <see cref="BranchVersionProvider"/>.
    /// </summary>
    public BranchVersionProvider(GitCli git)
    {
        _git = git;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<SemanticVersion> Get(string branchName)
    {
        IReadOnlyCollection<string> tags = _git.GetTagsOnBranch(branchName);
        return tags
            .Where(x => x.StartsWith("v"))
            .Select(SemanticVersion.Create)
            .OfType<SemanticVersion>()
            .ToArray();
    }
}
