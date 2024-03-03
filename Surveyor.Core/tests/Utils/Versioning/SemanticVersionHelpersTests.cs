using NUnit.Framework;
using Surveyor.Utils.Versioning;

namespace Surveyor.Core.Tests.Utils.Versioning;

[TestFixture]
internal class SemanticVersionHelpersTests
{
    [TestCase("1.2.3", "2.0.0")]
    [TestCase("0.2.3-alpha.1", "1.0.0")]
    [TestCase("0.2.3-alpha.1+BUILD", "1.0.0")]
    [TestCase("0.2.3", "1.0.0")]
    public void SemanticVersionHelpers_BumpMajor(string current, string expected)
    {
        // Arrange
        SemanticVersion version = SemanticVersion.Create(current) ?? throw new("Failed to parse version string");

        // Act
        SemanticVersion next = SemanticVersionHelpers.BumpMajor(version);

        // Assert
        Assert.That(next.ToString(), Is.EqualTo(expected));
    }

    [TestCase("1.2.3", "1.3.0")]
    [TestCase("0.2.3-alpha.1", "0.3.0")]
    [TestCase("0.2.3-alpha.1+BUILD", "0.3.0")]
    [TestCase("0.2.3", "0.3.0")]
    public void SemanticVersionHelpers_BumpMinor(string current, string expected)
    {
        // Arrange
        SemanticVersion version = SemanticVersion.Create(current) ?? throw new("Failed to parse version string");

        // Act
        SemanticVersion next = SemanticVersionHelpers.BumpMinor(version);

        // Assert
        Assert.That(next.ToString(), Is.EqualTo(expected));
    }

    [TestCase("1.2.3", "1.2.4")]
    [TestCase("0.2.3-alpha.1", "0.2.4")]
    [TestCase("0.2.3-alpha.1+BUILD", "0.2.4")]
    [TestCase("0.2.3", "0.2.4")]
    public void SemanticVersionHelpers_BumpPatch(string current, string expected)
    {
        // Arrange
        SemanticVersion version = SemanticVersion.Create(current) ?? throw new("Failed to parse version string");

        // Act
        SemanticVersion next = SemanticVersionHelpers.BumpPatch(version);

        // Assert
        Assert.That(next.ToString(), Is.EqualTo(expected));
    }
}
