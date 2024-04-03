namespace Surveyor.Versioning;

/// <summary>
/// A strategy to provide a collection of version tags pointing at the specified reference.
/// </summary>
public interface IHeadVersionProvider
{
    /// <summary>
    /// Get a collection of versions that point at <paramref name="branchName"/>.
    /// </summary>
    /// <param name="branchName">The name of the branch.</param>
    /// <returns>
    /// A collection of versions.
    /// </returns>
    public IReadOnlyCollection<SemanticVersion> Get(string branchName);
}


/// <inheritdoc />
public class HeadVersionProvider : IHeadVersionProvider
{
    private readonly GitCli _git;

    /// <summary>
    /// DI constructor for <see cref="HeadVersionProvider"/>.
    /// </summary>
    public HeadVersionProvider(GitCli git)
    {
        _git = git;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<SemanticVersion> Get(string branchName)
    {
        IReadOnlyCollection<string> tags = _git.GetTagsPointingAt(branchName);
        return tags
            .Where(x => x.StartsWith("v"))
            .Select(SemanticVersion.Create)
            .OfType<SemanticVersion>()
            .ToArray();
    }
}
