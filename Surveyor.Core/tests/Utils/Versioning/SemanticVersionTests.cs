using NUnit.Framework;
using Surveyor.Utils.Versioning;

namespace Surveyor.Core.Tests.Utils.Versioning;

[TestFixture]
internal class SemanticVersionTests
{
    [TestCase("1.2.3", 1, 2, 3)]
    [TestCase("1.2.3+HELLO", 1, 2, 3, "", "HELLO")]
    [TestCase("1.2.3-beta", 1, 2, 3, "beta")]
    [TestCase("1.2.3-beta.1", 1, 2, 3, "beta.1")]
    [TestCase("1.2.3-rc.1", 1, 2, 3, "rc.1")]
    [TestCase("1.2.3-beta+SPECIAL-BUILD", 1, 2, 3, "beta", "SPECIAL-BUILD")]
    [TestCase("1.2.3-4", 1, 2, 3, "4")]
    public void SemanticVersion_ToString(
        string expected,
        int major,
        int minor,
        int patch,
        string preRelease = "",
        string build = "")
    {
        // Arrange
        SemanticVersion version = new()
        {
            Major = major,
            Minor = minor,
            Patch = patch,
            PreRelease = preRelease,
            Build = build
        };

        // Act
        string result = version.ToString();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("1.2.3", 1, 2, 3)]
    [TestCase("1.2.3+HELLO", 1, 2, 3, "", "HELLO")]
    [TestCase("1.2.3-beta", 1, 2, 3, "beta")]
    [TestCase("1.2.3-beta.1", 1, 2, 3, "beta.1")]
    [TestCase("1.2.3-rc.1", 1, 2, 3, "rc.1")]
    [TestCase("1.2.3-beta+SPECIAL-BUILD", 1, 2, 3, "beta", "SPECIAL-BUILD")]
    [TestCase("1.2.3-4", 1, 2, 3, "4")]
    public void SemanticVersion_Create(
        string str,
        int major,
        int minor,
        int patch,
        string preRelease = "",
        string build = "")
    {
        // Arrange

        // Act
        SemanticVersion? result = SemanticVersion.Create(str);

        // Assert
        if (result is not SemanticVersion version)
        {
            Assert.Fail("Failed to parse version string");
            return;
        }
        Assert.That(version.Major, Is.EqualTo(major));
        Assert.That(version.Minor, Is.EqualTo(minor));
        Assert.That(version.Patch, Is.EqualTo(patch));
        Assert.That(version.PreRelease, Is.EqualTo(preRelease));
        Assert.That(version.Build, Is.EqualTo(build));
    }

    [Test]
    public void SemanticVersion_Comparable()
    {
        // Arrange
        string[] strings =
        [
            "0.0.1",
            "0.0.2",
            "0.1.0",
            "0.2.0",
            "1.0.0-alpha",
            "1.0.0-alpha.1",
            "1.0.0-alpha.beta",
            "1.0.0-beta",
            "1.0.0-beta.1",
            "1.0.0-beta.2",
            "1.0.0-beta.11",
            "1.0.0-beta.21",
            "1.0.0-rc.1",
            "1.0.0",
            "2.0.0",
            "3.0.0"
        ];
        SemanticVersion[] versions = strings
            .Select(SemanticVersion.Create)
            .OfType<SemanticVersion>()
            .ToArray();
        SemanticVersion[] shuffled = versions.OrderBy(_ => Guid.NewGuid()).ToArray();

        // Act
        SemanticVersion[] ordered = shuffled
            .OrderBy(x => x)
            .ToArray();

        // Assert
        Assert.That(ordered, Is.EqualTo(versions));
    }

    [Test]
    public void SemanticVersion_Comparable_Complex()
    {
        // Arrange
        string[] strings =
        [
            "0.0.4",
            "1.0.0-0A.is.legal",
            "1.0.0-alpha+beta",
            "1.0.0-alpha",
            "1.0.0-alpha.0valid",
            "1.0.0-alpha.1",
            "1.0.0-alpha.beta",
            "1.0.0-alpha.beta.1",
            "1.0.0-alpha-a.b-c-somethinglong+build.1-aef.1-its-okay",
            "1.0.0-alpha0.valid",
            "1.0.0-beta",
            "1.0.0-rc.1+build.1",
            "1.0.0+0.build.1-rc.10000aaa-kk-0.1",
            "1.0.0",
            "1.1.2-prerelease+meta",
            "1.1.2+meta",
            "1.1.2+meta-valid",
            "1.1.7",
            "1.2.3----R-S.12.9.1--.12+meta",
            "1.2.3----RC-SNAPSHOT.12.9.1--.12+788",
            "1.2.3----RC-SNAPSHOT.12.9.1--.12",
            "1.2.3-beta",
            "1.2.3-SNAPSHOT-123",
            "1.2.3",
            "2.0.0-rc.1+build.123",
            "2.0.0+build.1848",
            "2.0.0",
            "2.0.1-alpha.1227",
            "10.2.3-DEV-SNAPSHOT",
            "10.20.30"
        ];
        SemanticVersion[] versions = strings
            .Select(SemanticVersion.Create)
            .OfType<SemanticVersion>()
            .ToArray();
        SemanticVersion[] shuffled = versions.OrderBy(_ => Guid.NewGuid()).ToArray();

        // Act
        SemanticVersion[] ordered = shuffled
            .OrderBy(x => x)
            .ToArray();

        // Assert
        Assert.That(ordered, Is.EqualTo(versions));
    }
}
