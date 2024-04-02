using Surveyor.System;

namespace Surveyor.Versioning;

/// <summary>
/// Create release notes for all commits on the branch since the last release.
/// </summary>
/// <remarks>
/// Commit messages are expected to follow the <see href="https://www.conventionalcommits.org/en/v1.0.0/">Conventional Commits</see> specification.
/// </remarks>
public class ReleaseNotesActivity
{
    private readonly GitCli _git;
    private readonly IBranchVersionProvider _branchVersionProvider;
    private readonly IReleaseNotesFactory _releaseNotesFactory;

    /// <summary>
    /// DI constructor for <see cref="ReleaseNotesActivity"/>.
    /// </summary>
    public ReleaseNotesActivity(
        GitCli git,
        IBranchVersionProvider branchVersionProvider,
        IReleaseNotesFactory releaseNotesFactory)
    {
        _git = git;
        _branchVersionProvider = branchVersionProvider;
        _releaseNotesFactory = releaseNotesFactory;
    }

    /// <inheritdoc cref="ReleaseNotesActivity"/>
    /// <returns>The release notes formatted by <see cref="IReleaseNotesFactory"/>.</returns>
    public string Execute(ReleaseNotesActivityOptions options)
    {
        if (string.IsNullOrEmpty(options.Branch))
            options.Branch = _git.GetCurrentBranch();
        IReadOnlyCollection<SemanticVersion> branchVersions = _branchVersionProvider
            .Get(options.Branch)
            .OrderByDescending(x => x)
            .ToArray();
        SemanticVersion? latestReleaseQuery = branchVersions.FirstOrNull(x => !x.IsPreRelease());
        IReadOnlyCollection<ConventionalCommit> commits = latestReleaseQuery is SemanticVersion latestRelease
            ? _git.GetConventionalCommitsSince($"v{latestRelease}")
            : _git.GetAllConventionalCommits();
        return _releaseNotesFactory.Create(commits);
    }
}
