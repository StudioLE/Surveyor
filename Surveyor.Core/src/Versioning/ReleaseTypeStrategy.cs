using Surveyor.VersionControl;

namespace Surveyor.Versioning;

/// <summary>
/// A strategy to determine the release type.
/// </summary>
public interface IReleaseTypeStrategy
{
    /// <summary>
    /// Determine the release type based on the commits.
    /// </summary>
    /// <returns></returns>
    public ReleaseType Get();

    /// <summary>
    /// Determine the release type based on the commits since the specified version.
    /// </summary>
    /// <param name="latestVersion"></param>
    /// <returns></returns>
    public ReleaseType Get(SemanticVersion latestVersion);
}

/// <inheritdoc/>
public class ReleaseTypeStrategy : IReleaseTypeStrategy
{
    private readonly GitCli _git;

    /// <summary>
    /// The release type to use for breaking changes when the latest major version is 0.
    /// </summary>
    public ReleaseType VersionZeroBreakingChangeReleaseType { get; set; } = ReleaseType.Minor;

    /// <summary>
    /// DI constructor for <see cref="ReleaseTypeStrategy"/>.
    /// </summary>
    public ReleaseTypeStrategy(GitCli git)
    {
        _git = git;
    }

    /// <inheritdoc/>
    public ReleaseType Get()
    {
        IReadOnlyCollection<ConventionalCommit> commits = _git.GetAllConventionalCommits();
        ReleaseType releaseType = GetInternal(commits);
        if (releaseType == ReleaseType.Major)
            releaseType = VersionZeroBreakingChangeReleaseType;
        return releaseType;
    }

    /// <inheritdoc/>
    public ReleaseType Get(SemanticVersion latestVersion)
    {
        string sinceRef = $"v{latestVersion}";
        IReadOnlyCollection<ConventionalCommit> commits = _git.GetConventionalCommitsSince(sinceRef);
        ReleaseType releaseType = GetInternal(commits);
        if (releaseType == ReleaseType.Major && latestVersion.Major == 0)
            releaseType = VersionZeroBreakingChangeReleaseType;
        return releaseType;
    }

    private static ReleaseType GetInternal(IReadOnlyCollection<ConventionalCommit> commits)
    {
        if(commits.Count == 0)
            return ReleaseType.None;
        ReleaseType releaseType = commits
            .Select(x => x.Release)
            .Distinct()
            .Max();
        return releaseType;
    }
}

