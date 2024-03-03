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
        if(result is not SemanticVersion version)
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
}
