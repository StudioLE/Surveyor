using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Surveyor.Core.Tests.Resources;
using Surveyor.Hosting;
using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Versioning;

internal sealed class VersioningActivityTests
{
    private readonly IServiceProvider _services;

    public VersioningActivityTests()
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
    public async Task VersioningActivity_Execute_DI()
    {
        // Arrange
        VersioningActivity activity = _services.GetRequiredService<VersioningActivity>();
        IOptions<VersioningActivityOptions> options = _services.GetRequiredService<IOptions<VersioningActivityOptions>>();

        // Act
        SemanticVersion? version = await activity.Execute(options.Value);

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
    public async Task VersioningActivity_Execute(ReleaseType releaseType, string branchName, string? expected)
    {
        // Arrange
        ILogger<VersioningActivity> logger = _services.GetRequiredService<ILogger<VersioningActivity>>();
        MockPublishedVersionProvider publishedVersionProvider = new(CreateVersions([
            "0.1.0",
            "0.1.1",
            // "0.1.2",
            "1.0.0",
            "1.2.3",
            "1.2.4-alpha.1",
            "1.2.4-alpha.2",
            // "1.2.4-alpha.3",
            // "1.3.0",
            "2.0.0"
            // "2.1.0"
        ]));
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
        MockChangedFileProvider changedFileProvider = new([
            "Surveyor.Core/Versioning/VersioningActivity.cs"
        ]);
        ReleaseStreamProvider releaseStreamProvider = new();
        MockReleaseTypeStrategy releaseTypeStrategy = new(releaseType);
        VersioningActivityOptions options = _services.GetRequiredService<IOptions<VersioningActivityOptions>>().Value;
        options.Branch = branchName;
        VersioningActivity activity = new(
            logger,
            null!,
            publishedVersionProvider,
            repositoryVersionProvider,
            branchVersionProvider,
            changedFileProvider,
            releaseTypeStrategy,
            releaseStreamProvider);

        // Act
        SemanticVersion? version = await activity.Execute(options);

        // Assert
        if (expected is null)
            Assert.That(version, Is.Null);
        else
            Assert.That(version.ToString(), Is.EqualTo(expected));
    }

    private static SemanticVersion[] CreateVersions(string[] versions)
    {
        return versions
            .Select(SemanticVersion.Create)
            .OfType<SemanticVersion>()
            .ToArray();
    }
}
