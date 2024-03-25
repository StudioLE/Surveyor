using StudioLE.Extensions.System;
using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Resources;

internal static class TestHelpers
{
    private static readonly IReadOnlyCollection<ConventionalCommit> _commits = new List<ConventionalCommit>
    {
        new()
        {
            TypeId = "feat",
            Scope = "StudioLE.Example.Cli",
            Subject = "Added a new features to the CLI"
        },
        new()
        {
            TypeId = "fix",
            Scope = "StudioLE.Example.Cli",
            Subject = "prevent racing of requests",
            Body = "Introduce a request id and a reference to latest request. Dismiss\nincoming responses other than from latest request.\n\nRemove timeouts which were used to mitigate the racing issue but are\nobsolete now.",
            Footers = new Dictionary<string, string>
            {
                { "Reviewed-by", "Z" },
                { "HELLO-WORLD", "This is a\nmulti-line trailer" },
                { "Refs", "#123" }
            }
        },
        new()
        {
            TypeId = "fix",
            Scope = "StudioLE.Example.Core",
            Subject = "Fixed something in core.",
            Body = "Introduce a request id and a reference to latest request. Dismiss\nincoming responses other than from latest request.\n\nRemove timeouts which were used to mitigate the racing issue but are\nobsolete now.",
            Footers = new Dictionary<string, string>
            {
                { "Reviewed-by", "Z" },
                { "HELLO-WORLD", "This is a\nmulti-line trailer" },
                { "Refs", "#123" }
            }
        },
        new()
        {
            TypeId = "fix",
            Scope = "StudioLE.Example.Core",
            Subject = "Fixed something else in core.",
            Body = "Introduce a request id and a reference to latest request. Dismiss\nincoming responses other than from latest request.\n\nRemove timeouts which were used to mitigate the racing issue but are\nobsolete now.",
            Footers = new Dictionary<string, string>
            {
                { "Reviewed-by", "Z" },
                { "HELLO-WORLD", "This is a\nmulti-line trailer" },
                { "Refs", "#123" }
            }
        },
        new()
        {
            TypeId = "chore",
            Subject = "Updated dependencies"
        },
        new()
        {
            TypeId = "chore",
            Subject = "Updated dependencies",
            Body = "I have no idea why but lets describe why we're updating the dependencies..."
        },
        new()
        {
            TypeId = "docs",
            Subject = "correct spelling of CHANGELOG"
        },
        new()
        {
            TypeId = "feat",
            Subject = "allow provided config object to extend other configs",
            IsBreaking = true,
            Body = "`extends` key in config file is now used for extending other config files",
            Footers = new Dictionary<string, string>
            {
                { "BREAKING CHANGE", "`extends` key in config file is now used for extending other config files" }
            }
        },
        new()
        {
            TypeId = "hmmmm",
            Subject = "drop support for Node 6",
            Body = "use JavaScript features not available in Node 6."
        },
        new()
        {
            TypeId = "chore",
            IsBreaking = true,
            Subject = "drop support for Node 6",
            Body = "use JavaScript features not available in Node 6.",
            Footers = new Dictionary<string, string>
            {
                { "BREAKING CHANGE", "use JavaScript features not available in Node 6." }
            }
        }
    };


    public static IReadOnlyCollection<ConventionalCommit> ExampleCommits()
    {
        return _commits
            .Select((commit, i) => commit with
            {
                Hash = Enumerable.Repeat(i + "a", 4).Join(string.Empty)
            })
            .ToArray();
    }
}
