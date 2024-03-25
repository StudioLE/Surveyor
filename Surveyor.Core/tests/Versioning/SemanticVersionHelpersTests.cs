using NUnit.Framework;
using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Versioning;

[TestFixture]
internal class SemanticVersionHelpersTests
{
    [TestCase("0.2.3-alpha.1", "1.0.0")]
    [TestCase("0.2.3-alpha.1+BUILD", "1.0.0+BUILD")]
    [TestCase("0.2.3", "1.0.0")]
    [TestCase("1.0.0-alpha.1", "1.0.0")]
    [TestCase("1.0.0", "2.0.0")]
    [TestCase("1.1.0-alpha.1", "2.0.0")]
    [TestCase("1.2.3", "2.0.0")]
    public void SemanticVersionHelpers_BumpMajor(string current, string expected)
    {
        // Arrange
        SemanticVersion version = SemanticVersion.Create(current) ?? throw new("Failed to parse version string");

        // Act
        SemanticVersion next = SemanticVersionHelpers.BumpMajor(version);

        // Assert
        Assert.That(next.ToString(), Is.EqualTo(expected));
    }

    [TestCase("0.2.3-alpha.1", "0.3.0")]
    [TestCase("0.2.3-alpha.1+BUILD", "0.3.0+BUILD")]
    [TestCase("0.2.3", "0.3.0")]
    [TestCase("1.0.0-alpha.1", "1.0.0")]
    [TestCase("1.0.0", "1.1.0")]
    [TestCase("1.1.0-alpha.1", "1.1.0")]
    [TestCase("1.2.3", "1.3.0")]
    public void SemanticVersionHelpers_BumpMinor(string current, string expected)
    {
        // Arrange
        SemanticVersion version = SemanticVersion.Create(current) ?? throw new("Failed to parse version string");

        // Act
        SemanticVersion next = SemanticVersionHelpers.BumpMinor(version);

        // Assert
        Assert.That(next.ToString(), Is.EqualTo(expected));
    }

    [TestCase("0.2.3-alpha.1", "0.2.3")]
    [TestCase("0.2.3-alpha.1+BUILD", "0.2.3+BUILD")]
    [TestCase("0.2.3", "0.2.4")]
    [TestCase("1.0.0-alpha.1", "1.0.0")]
    [TestCase("1.0.0", "1.0.1")]
    [TestCase("1.1.0-alpha.1", "1.1.0")]
    [TestCase("1.2.3", "1.2.4")]
    public void SemanticVersionHelpers_BumpPatch(string current, string expected)
    {
        // Arrange
        SemanticVersion version = SemanticVersion.Create(current) ?? throw new("Failed to parse version string");

        // Act
        SemanticVersion next = SemanticVersionHelpers.BumpPatch(version);

        // Assert
        Assert.That(next.ToString(), Is.EqualTo(expected));
    }

    [TestCase("0.2.3-alpha.1", "0.2.3-alpha.2")]
    [TestCase("0.2.3-alpha.1+BUILD", "0.2.3-alpha.2+BUILD")]
    [TestCase("0.2.3", "0.2.3-alpha.1")]
    [TestCase("1.0.0-alpha.1", "1.0.0-alpha.2")]
    [TestCase("1.0.0", "1.0.0-alpha.1")]
    [TestCase("1.1.0-alpha.1", "1.1.0-alpha.2")]
    [TestCase("1.2.3", "1.2.3-alpha.1")]
    [TestCase("0.2.3-alpha.4", "0.2.3-alpha.5")]
    [TestCase("0.2.3-alpha", "0.2.3-alpha.1")]
    [TestCase("0.2.3-alpha.4+BUILD", "0.2.3-alpha.5+BUILD")]
    [TestCase("0.2.3-release.4.5", "0.2.3-alpha.1")]
    public void SemanticVersionHelpers_BumpPreRelease(string current, string expected)
    {
        // Arrange
        SemanticVersion version = SemanticVersion.Create(current) ?? throw new("Failed to parse version string");

        // Act
        SemanticVersion next = SemanticVersionHelpers.BumpPreRelease(version, "alpha");

        // Assert
        Assert.That(next.ToString(), Is.EqualTo(expected));
    }

    [TestCase("0.0.0-release.4.5", ReleaseType.Major)]
    [TestCase("0.0.0+BUILD", ReleaseType.Major)]
    [TestCase("10.0.0+BUILD", ReleaseType.Major)]
    [TestCase("10.0.0+BUILD", ReleaseType.Major)]
    [TestCase("0.1.0+BUILD", ReleaseType.Minor)]
    [TestCase("10.8.0-release.4.5+BUILD", ReleaseType.Minor)]
    [TestCase("10.8.0+BUILD", ReleaseType.Minor)]
    [TestCase("1.2.3", ReleaseType.Patch)]
    [TestCase("0.2.3-alpha.4", ReleaseType.Patch)]
    [TestCase("0.2.3-alpha", ReleaseType.Patch)]
    [TestCase("0.2.3-alpha.4+BUILD", ReleaseType.Patch)]
    [TestCase("0.2.3-release.4.5", ReleaseType.Patch)]
    public void SemanticVersionHelpers_GetReleaseType(string current, ReleaseType expected)
    {
        // Arrange
        SemanticVersion version = SemanticVersion.Create(current) ?? throw new("Failed to parse version string");

        // Act
        ReleaseType result = version.GetReleaseType();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}
