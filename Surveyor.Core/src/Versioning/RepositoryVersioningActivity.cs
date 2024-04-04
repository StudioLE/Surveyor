using Microsoft.Extensions.Logging;
using Surveyor.System;

namespace Surveyor.Versioning;

/// <summary>
/// Automatically determine the version of the package by:
/// <para>Querying the package feed for the latest version</para>
/// <para>Determining if there are any changes since</para>
/// <para>Analyzing the commit messages to determine the appropriate release type</para>
/// <para>Incrementing the version based on the release type</para>
/// </summary>
public class RepositoryVersioningActivity
{
    private readonly ILogger<RepositoryVersioningActivity> _logger;
    private readonly GitCli _git;
    private readonly IRepositoryVersionProvider _repositoryVersionProvider;
    private readonly IBranchVersionProvider _branchVersionProvider;
    private readonly IHeadVersionProvider _headVersionProvider;
    private readonly IReleaseTypeStrategy _releaseTypeStrategy;
    private readonly IReleaseStreamProvider _releaseStreamProvider;

    /// <summary>
    /// DI constructor for <see cref="RepositoryVersioningActivity"/>.
    /// </summary>
    public RepositoryVersioningActivity(
        ILogger<RepositoryVersioningActivity> logger,
        GitCli git,
        IRepositoryVersionProvider repositoryVersionProvider,
        IBranchVersionProvider branchVersionProvider,
        IHeadVersionProvider headVersionProvider,
        IReleaseTypeStrategy releaseTypeStrategy,
        IReleaseStreamProvider releaseStreamProvider)

    {
        _logger = logger;
        _git = git;
        _repositoryVersionProvider = repositoryVersionProvider;
        _branchVersionProvider = branchVersionProvider;
        _headVersionProvider = headVersionProvider;
        _releaseTypeStrategy = releaseTypeStrategy;
        _releaseStreamProvider = releaseStreamProvider;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public SemanticVersion? Execute(VersioningActivityOptions options)
    {
        if (options.Package != string.Empty)
        {
            _logger.LogError($"The {nameof(options.Package)} option should be empty.");
            return null;
        }
        if (string.IsNullOrEmpty(options.Directory))
            options.Directory = Directory.GetCurrentDirectory();
        if (!Directory.Exists(options.Directory))
        {
            _logger.LogError($"The {nameof(options.Directory)} does not exist: {options.Directory}.");
            return null;
        }
        if(_git.RootDirectory != options.Directory)
            _git.SetRootDirectory(options.Directory);
        if (string.IsNullOrEmpty(options.Branch))
            options.Branch = _git.GetCurrentBranch();
        ReleaseStream? releaseStreamQuery = _releaseStreamProvider.Get(options.Branch);
        if (releaseStreamQuery is not ReleaseStream releaseStream)
        {
            _logger.LogInformation($"The {options.Branch} branch is not a release stream.");
            return null;
        }
        IReadOnlyCollection<SemanticVersion> branchVersions = _branchVersionProvider
            .Get(options.Branch)
            .OrderByDescending(x => x)
            .ToArray();
        SemanticVersion? latestFullVersionOnBranch = branchVersions.FirstOrNull(x => !x.IsPreRelease());
        _logger.LogDebug($"Last version on branch: {latestFullVersionOnBranch}.");
        ReleaseType releaseType = branchVersions.Count == 0
            ? _releaseTypeStrategy.Get()
            : _releaseTypeStrategy.Get(latestFullVersionOnBranch!.Value);
        _logger.LogDebug($"Release type: {releaseType}.");
        IReadOnlyCollection<SemanticVersion> repositoryVersions = _repositoryVersionProvider.Get();
        IReadOnlyCollection<SemanticVersion> headVersions = _headVersionProvider.Get(options.Branch);
        SemanticVersion version = BumpFullVersion(branchVersions, repositoryVersions, headVersions, releaseType);
        return !releaseStream.IsPreRelease
            ? version
            : BumpPreReleaseVersion(version, repositoryVersions, headVersions, releaseStream);
    }

    private static SemanticVersion BumpFullVersion(
        IReadOnlyCollection<SemanticVersion> branchVersions,
        IReadOnlyCollection<SemanticVersion> repositoryVersions,
        IReadOnlyCollection<SemanticVersion> headVersions,
        ReleaseType releaseType)
    {
        SemanticVersion version = branchVersions.Count == 0
            ? new()
            : branchVersions.First();
        ReleaseType latestReleaseType = version.GetReleaseType();
        if (version.IsPreRelease())
            version = new()
            {
                Major = version.Major,
                Minor = version.Minor,
                Patch = version.Patch
            };
        if (releaseType > latestReleaseType)
            version = SemanticVersionHelpers.Bump(version, releaseType);
        while (repositoryVersions.Contains(version) && !headVersions.Contains(version))
            version = SemanticVersionHelpers.Bump(version, releaseType);
        return version;
    }

    private static SemanticVersion BumpPreReleaseVersion(
        SemanticVersion version,
        IReadOnlyCollection<SemanticVersion> repositoryVersions,
        IReadOnlyCollection<SemanticVersion> headVersions,
        ReleaseStream releaseStream)
    {
        do
            version = SemanticVersionHelpers.BumpPreRelease(version, releaseStream.Id);
        while (repositoryVersions.Contains(version) && !headVersions.Contains(version));
        return version;
    }
}
