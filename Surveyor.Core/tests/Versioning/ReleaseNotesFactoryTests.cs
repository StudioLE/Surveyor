using Microsoft.Extensions.Options;
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
    private readonly IContext _context = new NUnitContext();

    [Test]
    public async Task ReleaseNotesFactory_Create([Values] bool groupByScope)
    {
        // Arrange
        IReadOnlyCollection<ConventionalCommit> commits = TestHelpers.ExampleCommits();
        ReleaseNotesActivityOptions options = new()
        {
            GroupByScope = groupByScope
        };
        ReleaseNotesFactory factory = new(Options.Create(options), new());

        // Act
        string result = factory.Create(commits);

        // Assert
        Assert.That(result, Is.Not.Null);
        await _context.Verify(result);
    }
}
