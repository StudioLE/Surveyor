using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Verify;
using Surveyor.Core.Tests.Resources;
using Surveyor.ReleaseNotes;
using Surveyor.VersionControl;

namespace Surveyor.Core.Tests.Versioning;

[TestFixture]
internal class ReleaseNotesFactoryTests
{
    private readonly ReleaseNotesByScopeFactory _factory = new(new());
    private readonly IContext _context = new NUnitContext();

    [Test]
    public async Task ReleaseNotesFactory_Create()
    {
        // Arrange
        IReadOnlyCollection<ConventionalCommit> commits = TestHelpers.ExampleCommits();

        // Act
        string result = _factory.Create(commits);

        // Assert
        Assert.That(result, Is.Not.Null);
        await _context.Verify(result);
    }
}
