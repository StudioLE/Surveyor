using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using Surveyor.Packages;
using Surveyor.System;
using Surveyor.Utils.Versioning;
using Task = Microsoft.Build.Utilities.Task;

namespace Surveyor.Build;

/// <summary>
/// A build task to determine the latest version of a NuGet package by querying the package feed.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/visualstudio/msbuild/tutorial-custom-task-code-generation"/>
/// <seealso href="https://learn.microsoft.com/en-us/visualstudio/msbuild/task-writing"/>
public class VersioningTask : Task
{
    /// <summary>
    /// The name of the package.
    /// </summary>
    public string PackageName { get; set; } = string.Empty;

    /// <summary>
    /// The URL of the package feed.
    /// </summary>
    public string Feed { get; set; } = string.Empty;

    /// <summary>
    /// The auth token for the package feed.
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;

    /// <summary>
    /// The latest version of the package.
    /// </summary>
    [Output]
    public string Latest { get; set; } = string.Empty;

    /// <summary>
    /// The next major version of the package.
    /// </summary>
    [Output]
    public string NextMajor { get; set; } = string.Empty;

    /// <summary>
    /// The next minor version of the package.
    /// </summary>
    [Output]
    public string NextMinor { get; set; } = string.Empty;

    /// <summary>
    /// The next patch version of the package.
    /// </summary>
    [Output]
    public string NextPatch { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override bool Execute()
    {
        if(string.IsNullOrEmpty(PackageName))
        {
            Log.LogError("The package name is required.");
            return false;
        }
        PackageApiOptions options = new()
        {
            Feed = Feed,
            AuthToken = AuthToken
        };
        PackageApi api = new(new NullLogger<PackageApi>(), options);
        Task<IReadOnlyCollection<SemanticVersion>> task = api.GetPackageVersions(PackageName, false);
        task.Wait();
        IReadOnlyCollection<SemanticVersion> versions = task.Result;
        SemanticVersion? latestQuery = versions.FirstOrNull();
        if(latestQuery is not SemanticVersion latest)
        {
            Log.LogError("Failed to get the version.");
            return false;
        }
        Latest = latest.ToString();
        NextMajor = SemanticVersionHelpers.BumpMajor(latest).ToString();
        NextMinor = SemanticVersionHelpers.BumpMinor(latest).ToString();
        NextPatch = SemanticVersionHelpers.BumpPatch(latest).ToString();
        return true;

    }
}
