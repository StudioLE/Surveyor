using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Surveyor.Core.Tests.Resources;
using Surveyor.Hosting;
using Surveyor.VersionControl;
using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Versioning;

internal sealed class RepositoryVersioningActivityTests
{
    private readonly IServiceProvider _services;

    public RepositoryVersioningActivityTests()
    {
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddSurveyorServices()
                .AddSurveyorOptions())
            .Build();
        _services = host.Services;
    }

    [Test]
    [Explicit("Requires configuration")]
    public void RepositoryVersioningActivity_Execute_DI()
    {
        // Arrange
        RepositoryVersioningActivity activity = _services.GetRequiredService<RepositoryVersioningActivity>();
        IOptions<VersioningActivityOptions> options = _services.GetRequiredService<IOptions<VersioningActivityOptions>>();

        // Act
        SemanticVersion? version = activity.Execute(options.Value);

        // Assert
        Assert.That(version, Is.Not.Null);
    }

    [TestCase(ReleaseType.Patch, "main", "1.2.4")]
    [TestCase(ReleaseType.Patch, "alpha", "1.2.4-alpha.4")]
    [TestCase(ReleaseType.Patch, "beta", "1.2.4-beta.1")]
    [TestCase(ReleaseType.Patch, "rc", "1.2.4-rc.1")]
    // [TestCase(ReleaseType.Patch, "v2", null)]
    // [TestCase(ReleaseType.Patch, "v2.2", null)]
    // [TestCase(ReleaseType.Patch, "v3", "3.0.0")]
    [TestCase(ReleaseType.Patch, "wip", null)]
    [TestCase(ReleaseType.Minor, "main", "1.4.0")]
    [TestCase(ReleaseType.Major, "main", "3.0.0")]
    public void RepositoryVersioningActivity_Execute(ReleaseType releaseType, string branchName, string? expected)
    {
        // Arrange
        ILogger<RepositoryVersioningActivity> logger = _services.GetRequiredService<ILogger<RepositoryVersioningActivity>>();
        MockRepositoryVersionProvider repositoryVersionProvider = new(CreateVersions([
            "0.1.0",
            "0.1.1",
            "0.1.2",
            "1.0.0",
            "1.2.3",
            "1.2.4-alpha.1",
            "1.2.4-alpha.2",
            "1.2.4-alpha.3",
            "1.3.0",
            "2.0.0",
            "2.1.0"
        ]));
        MockBranchVersionProvider branchVersionProvider = new(CreateVersions([
            "0.1.0",
            "0.1.1",
            "0.1.2",
            "1.0.0",
            "1.2.3",
            "1.2.4-alpha.1",
            "1.2.4-alpha.2"
            // "1.2.4-alpha.3"
            // "1.3.0",
            // "2.0.0",
            // "2.1.0"
        ]));
        MockHeadVersionProvider headVersionProvider = new([]);
        ReleaseStreamProvider releaseStreamProvider = new();
        MockReleaseTypeStrategy releaseTypeStrategy = new(releaseType);
        VersioningActivityOptions options = _services.GetRequiredService<IOptions<VersioningActivityOptions>>().Value;
        options.Branch = branchName;
        options.Directory = Path.GetTempPath();
        options.Package = string.Empty;
        GitCli git = new(new GitCliOptions
        {
            Directory = options.Directory,
            SkipValidation = true
        });
        RepositoryVersioningActivity activity = new(
            logger,
            git,
            repositoryVersionProvider,
            branchVersionProvider,
            headVersionProvider,
            releaseTypeStrategy,
            releaseStreamProvider);

        // Act
        SemanticVersion? version = activity.Execute(options);

        // Assert
        if (expected is null)
            Assert.That(version, Is.Null);
        else
            Assert.That(version.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void RepositoryVersioningActivity_Execute_NoProjectChanges_HeadNotTagged()
    {
        // Arrange
        ILogger<RepositoryVersioningActivity> logger = _services.GetRequiredService<ILogger<RepositoryVersioningActivity>>();
        MockRepositoryVersionProvider repositoryVersionProvider = new(CreateVersions([
            "0.1.0",
            "0.1.1",
            "0.1.2"
        ]));
        MockBranchVersionProvider branchVersionProvider = new(CreateVersions([
            "0.1.0",
            "0.1.1",
            "0.1.2"
        ]));
        MockHeadVersionProvider headVersionProvider = new([]);
        ReleaseStreamProvider releaseStreamProvider = new();
        MockReleaseTypeStrategy releaseTypeStrategy = new(ReleaseType.Patch);
        VersioningActivityOptions options = _services.GetRequiredService<IOptions<VersioningActivityOptions>>().Value;
        options.Branch = "alpha";
        options.Directory = Path.GetTempPath();
        options.Package = string.Empty;
        GitCli git = new(new GitCliOptions
        {
            Directory = options.Directory,
            SkipValidation = true
        });
        RepositoryVersioningActivity activity = new(
            logger,
            git,
            repositoryVersionProvider,
            branchVersionProvider,
            headVersionProvider,
            releaseTypeStrategy,
            releaseStreamProvider);

        // Act
        SemanticVersion? version = activity.Execute(options);

        // Assert
        Assert.That(version.ToString(), Is.EqualTo("0.1.3-alpha.1"));
    }

    [Test]
    public void VersioningActivity_Execute_HeadTaggedButNotPublished()
    {
        // Arrange
        ILogger<RepositoryVersioningActivity> logger = _services.GetRequiredService<ILogger<RepositoryVersioningActivity>>();
        MockRepositoryVersionProvider repositoryVersionProvider = new(CreateVersions([
            "0.1.0",
            "0.1.1",
            "0.1.2"
        ]));
        MockBranchVersionProvider branchVersionProvider = new(CreateVersions([
            "0.1.0",
            "0.1.1",
            "0.1.2"
        ]));
        MockHeadVersionProvider headVersionProvider = new(CreateVersions([
            "0.1.2"
        ]));
        ReleaseStreamProvider releaseStreamProvider = new();
        MockReleaseTypeStrategy releaseTypeStrategy = new(ReleaseType.Patch);
        VersioningActivityOptions options = _services.GetRequiredService<IOptions<VersioningActivityOptions>>().Value;
        options.Branch = "main";
        options.Directory = Path.GetTempPath();
        options.Package = string.Empty;
        GitCli git = new(new GitCliOptions
        {
            Directory = options.Directory,
            SkipValidation = true
        });
        RepositoryVersioningActivity activity = new(
            logger,
            git,
            repositoryVersionProvider,
            branchVersionProvider,
            headVersionProvider,
            releaseTypeStrategy,
            releaseStreamProvider);

        // Act
        SemanticVersion? version = activity.Execute(options);

        // Assert
        Assert.That(version.ToString(), Is.EqualTo("0.1.2"));
    }

    [Test]
    public void VersioningActivity_Execute_NoProjectChanges_HeadTaggedPreRelease()
    {
        // Arrange
        ILogger<RepositoryVersioningActivity> logger = _services.GetRequiredService<ILogger<RepositoryVersioningActivity>>();
        MockRepositoryVersionProvider repositoryVersionProvider = new(CreateVersions([
            "0.1.0",
            "0.1.1",
            "0.1.2-alpha.1"
        ]));
        MockBranchVersionProvider branchVersionProvider = new(CreateVersions([
            "0.1.0",
            "0.1.1",
            "0.1.2-alpha.1"
        ]));
        MockHeadVersionProvider headVersionProvider = new(CreateVersions([
            "0.1.2-alpha.1"
        ]));
        ReleaseStreamProvider releaseStreamProvider = new();
        MockReleaseTypeStrategy releaseTypeStrategy = new(ReleaseType.Patch);
        VersioningActivityOptions options = _services.GetRequiredService<IOptions<VersioningActivityOptions>>().Value;
        options.Branch = "main";
        options.Directory = Path.GetTempPath();
        options.Package = string.Empty;
        GitCli git = new(new GitCliOptions
        {
            Directory = options.Directory,
            SkipValidation = true
        });
        RepositoryVersioningActivity activity = new(
            logger,
            git,
            repositoryVersionProvider,
            branchVersionProvider,
            headVersionProvider,
            releaseTypeStrategy,
            releaseStreamProvider);

        // Act
        SemanticVersion? version = activity.Execute(options);

        // Assert
        Assert.That(version.ToString(), Is.EqualTo("0.1.2"));
    }

    private static SemanticVersion[] CreateVersions(string[] versions)
    {
        return versions
            .Select(SemanticVersion.Create)
            .OfType<SemanticVersion>()
            .ToArray();
    }
}
