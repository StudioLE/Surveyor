using Microsoft.Extensions.Options;
using NUnit.Framework;
using Surveyor.VersionControl;

namespace Surveyor.Core.Tests.Versioning;

[TestFixture]
internal class GitCliTests
{
    private const string SinceRef = "v0.0.0";
    private const string RootDirectory = "../../../../../";
    private readonly GitCli _git = new(Options.Create<GitCliOptions>(new() { Directory = RootDirectory }));

    [Test]
    [Explicit("Requires fetched tags")]
    public void GitCli_GetTags()
    {
        // Arrange
        // Act
        IReadOnlyCollection<string> tags = _git.GetTags();

        // Assert
        Assert.That(tags, Is.Not.Empty);
    }

    [Test]
    [Explicit("Requires fetched tags")]
    public void GitCli_GetProjectsWithChanges()
    {
        // Arrange
        // Act
        IReadOnlyCollection<string> results = _git.GetProjectsWithChangesSince(SinceRef);

        // Assert
        Assert.That(results, Is.Not.Empty);
    }

    [Test]
    [Explicit("Requires fetched tags")]
    public void GitCli_GetConventionalCommitsSince()
    {
        // Arrange
        // Act
        IReadOnlyCollection<ConventionalCommit> results = _git.GetConventionalCommitsSince(SinceRef);

        // Assert
        Assert.That(results, Is.Not.Empty);
    }
}
