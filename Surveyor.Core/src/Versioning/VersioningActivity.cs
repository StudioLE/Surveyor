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
public class VersioningActivity
{
    private readonly ILogger<VersioningActivity> _logger;
    private readonly GitCli _git;
    private readonly IPublishedVersionProvider _publishedVersionProvider;
    private readonly IRepositoryVersionProvider _repositoryVersionProvider;
    private readonly IBranchVersionProvider _branchVersionProvider;
    private readonly IHeadVersionProvider _headVersionProvider;
    private readonly IChangedFileProvider _changedFileProvider;
    private readonly IReleaseTypeStrategy _releaseTypeStrategy;
    private readonly IReleaseStreamProvider _releaseStreamProvider;

    /// <summary>
    /// DI constructor for <see cref="VersioningActivity"/>.
    /// </summary>
    public VersioningActivity(
        ILogger<VersioningActivity> logger,
        GitCli git,
        IPublishedVersionProvider publishedVersionProvider,
        IRepositoryVersionProvider repositoryVersionProvider,
        IBranchVersionProvider branchVersionProvider,
        IHeadVersionProvider headVersionProvider,
        IChangedFileProvider changedFileProvider,
        IReleaseTypeStrategy releaseTypeStrategy,
        IReleaseStreamProvider releaseStreamProvider)

    {
        _logger = logger;
        _git = git;
        _publishedVersionProvider = publishedVersionProvider;
        _repositoryVersionProvider = repositoryVersionProvider;
        _branchVersionProvider = branchVersionProvider;
        _headVersionProvider = headVersionProvider;
        _changedFileProvider = changedFileProvider;
        _releaseTypeStrategy = releaseTypeStrategy;
        _releaseStreamProvider = releaseStreamProvider;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public async Task<SemanticVersion?> Execute(VersioningActivityOptions options)
    {
        if (string.IsNullOrEmpty(options.Branch))
            options.Branch = _git.GetCurrentBranch();
        if (string.IsNullOrEmpty(options.Directory))
            options.Directory = _git.RootDirectory;
        ReleaseStream? releaseStreamQuery = _releaseStreamProvider.Get(options.Branch);
        if (releaseStreamQuery is not ReleaseStream releaseStream)
        {
            _logger.LogInformation($"[{options.Package}] The {options.Branch} branch is not a release stream.");
            return null;
        }
        IReadOnlyCollection<SemanticVersion> branchVersions = _branchVersionProvider
            .Get(options.Branch)
            .OrderByDescending(x => x)
            .ToArray();
        return string.IsNullOrEmpty(options.Package)
            ? ExecuteForAllProjects(options, releaseStream, branchVersions)
            : await ExecuteForSingleProject(options, releaseStream, branchVersions);
    }

    private async Task<SemanticVersion?> ExecuteForSingleProject(
        VersioningActivityOptions options,
        ReleaseStream releaseStream,
        IReadOnlyCollection<SemanticVersion> branchVersions)
    {
        IReadOnlyCollection<SemanticVersion> publishedVersions = (await _publishedVersionProvider.Get(options.Package))
            .OrderByDescending(x => x)
            .ToArray();
        IReadOnlyCollection<SemanticVersion> publishedVersionsOnBranch = publishedVersions
            .Where(x => branchVersions.Contains(x))
            .ToArray();
        SemanticVersion? latestPublishedVersionOnBranch = publishedVersionsOnBranch.FirstOrNull();
        IReadOnlyCollection<string> changedFiles = publishedVersionsOnBranch.Count == 0
            ? _changedFileProvider.Get(options.Directory).ToArray()
            : _changedFileProvider.Get(options.Directory, latestPublishedVersionOnBranch!.Value).ToArray();
        _logger.LogDebug($"[{options.Package}] Last published version on branch: {latestPublishedVersionOnBranch}.");
        if (!changedFiles.Any())
        {
            _logger.LogInformation($"[{options.Package}] No changes have been made since the last published version.");
            return latestPublishedVersionOnBranch;
        }
        _logger.LogDebug($"[{options.Package}] {changedFiles.Count} files have changed since the last published version.");
        ReleaseType releaseType = publishedVersionsOnBranch.Count == 0
            ? _releaseTypeStrategy.Get()
            : _releaseTypeStrategy.Get(latestPublishedVersionOnBranch!.Value);
        _logger.LogDebug($"[{options.Package}] Release type: {releaseType}.");
        IReadOnlyCollection<SemanticVersion> repositoryVersions = _repositoryVersionProvider.Get();
        IReadOnlyCollection<SemanticVersion> headVersions = _headVersionProvider.Get(options.Branch);
        SemanticVersion version = BumpFullVersion(branchVersions, repositoryVersions, headVersions, releaseType);
        return !releaseStream.IsPreRelease
            ? version
            : BumpPreReleaseVersion(version, repositoryVersions, headVersions, releaseStream);
    }

    private SemanticVersion? ExecuteForAllProjects(
        VersioningActivityOptions options,
        ReleaseStream releaseStream,
        IReadOnlyCollection<SemanticVersion> branchVersions)
    {
        SemanticVersion? latestFullVersionOnBranch = branchVersions.FirstOrNull(x => !x.IsPreRelease());
        _logger.LogDebug($"[{options.Package}] Last version on branch: {latestFullVersionOnBranch}.");
        ReleaseType releaseType = branchVersions.Count == 0
            ? _releaseTypeStrategy.Get()
            : _releaseTypeStrategy.Get(latestFullVersionOnBranch!.Value);
        _logger.LogDebug($"[{options.Package}] Release type: {releaseType}.");
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
