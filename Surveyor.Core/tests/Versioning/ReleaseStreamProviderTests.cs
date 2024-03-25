using NUnit.Framework;
using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Versioning;

[TestFixture]
internal class ReleaseStreamProviderTests
{
    [TestCase("alpha", "alpha")]
    [TestCase("beta", "beta")]
    [TestCase("main", "main")]
    [TestCase("v1", "major")]
    [TestCase("v1.2", "minor")]
    [TestCase("v1.2.3", "patch")]
    public void ReleaseStreamProvider_Get(string branchName, string expected)
    {
        // Arrange
        ReleaseStreamProvider provider = new();

        // Act
        ReleaseStream? result = provider.Get(branchName);

        // Assert
        if(result is ReleaseStream releaseStream)
            Assert.That(releaseStream.Id, Is.EqualTo(expected));
        else
            Assert.Fail();
    }
}
