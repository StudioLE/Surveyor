using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Verify.Yaml;
using Surveyor.VersionControl;

namespace Surveyor.Core.Tests.Versioning;

[TestFixture]
internal class ConventionalCommitParserTests
{
    private readonly ConventionalCommitParser _parser = new(new());
    private readonly IContext _context = new NUnitContext();

    [Test]
    public async Task ConventionalCommitParser_Parse_Standard()
    {
        // Arrange
        const string message = "feat(api): send an email to the customer when a product is shipped";

        // Act
        ConventionalCommit? result = _parser.Parse(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        await _context.VerifyAsYaml(result!);
    }

    [Test]
    public async Task ConventionalCommitParser_Parse_WithBreakingChange()
    {
        // Arrange
        const string message = "feat(api)!: send an email to the customer when a product is shipped";

        // Act
        ConventionalCommit? result = _parser.Parse(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        await _context.VerifyAsYaml(result!);
    }

    [Test]
    public async Task ConventionalCommitParser_Parse_WithNoScope()
    {
        // Arrange
        const string message = "docs: correct spelling of CHANGELOG";

        // Act
        ConventionalCommit? result = _parser.Parse(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        await _context.VerifyAsYaml(result!);
    }

    [Test]
    public async Task ConventionalCommitParser_Parse_WithBreakingChangeFooter()
    {
        // Arrange
        const string message = """
                               feat: allow provided config object to extend other configs

                               BREAKING CHANGE: `extends` key in config file is now used for extending other config files
                               """;

        // Act
        ConventionalCommit? result = _parser.Parse(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        await _context.VerifyAsYaml(result!);
    }

    [Test]
    public async Task ConventionalCommitParser_Parse_WithBreakingChangeAndFooter()
    {
        // Arrange
        const string message = """
                               chore!: drop support for Node 6

                               BREAKING CHANGE: use JavaScript features not available in Node 6.
                               """;

        // Act
        ConventionalCommit? result = _parser.Parse(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        await _context.VerifyAsYaml(result!);
    }

    [Test]
    public async Task ConventionalCommitParser_Parse_WithUnknownScope()
    {
        // Arrange
        const string message = """
                               hmmmm: drop support for Node 6

                               use JavaScript features not available in Node 6.
                               """;

        // Act
        ConventionalCommit? result = _parser.Parse(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        await _context.VerifyAsYaml(result!);
    }

    [Test]
    public async Task ConventionalCommitParser_Parse_WithBodyAndFooters()
    {
        // Arrange
        const string message = """
                               fix: prevent racing of requests

                               Introduce a request id and a reference to latest request. Dismiss
                               incoming responses other than from latest request.

                               Remove timeouts which were used to mitigate the racing issue but are
                               obsolete now.

                               Reviewed-by: Z
                               HELLO-WORLD: This is a
                               multi-line trailer
                               Refs: #123
                               """;

        // Act
        ConventionalCommit? result = _parser.Parse(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        await _context.VerifyAsYaml(result!);
    }
}
