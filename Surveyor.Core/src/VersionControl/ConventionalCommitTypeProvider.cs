using Surveyor.System;
using Surveyor.Versioning;

namespace Surveyor.VersionControl;

/// <summary>
/// A provider for <see cref="ConventionalCommitType"/>
/// </summary>
public class ConventionalCommitTypeProvider
{
    private readonly IReadOnlyCollection<ConventionalCommitType> _types;

    /// <summary>
    /// Creates a new instance of <see cref="ConventionalCommitTypeProvider"/>.
    /// </summary>
    public ConventionalCommitTypeProvider()
    {
        _types = DefaultTypes();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ConventionalCommitTypeProvider"/>.
    /// </summary>
    public ConventionalCommitTypeProvider(IReadOnlyCollection<ConventionalCommitType> types)
    {
        _types = types;
    }

    /// <summary>
    /// Get a <see cref="ConventionalCommitType"/> by its unique identifier or alternative identifier.
    /// </summary>
    /// <param name="id">The id of the type to retrieve.</param>
    /// <returns>
    /// The <see cref="ConventionalCommitType"/> if found; otherwise, <see langword="null"/>.
    /// </returns>
    public ConventionalCommitType? Get(string id)
    {
        ConventionalCommitType? idQuery = _types
            .FirstOrNull(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (idQuery is ConventionalCommitType)
            return idQuery;
        return _types
            .FirstOrNull(type => type
                .AlternativeIds
                .Any(altId => altId
                    .Equals(id, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Get the default types.
    /// </summary>
    public IReadOnlyCollection<ConventionalCommitType> DefaultTypes()
    {
        return new ConventionalCommitType[]
        {
            new()
            {
                Id = "major",
                Name = "Major",
                Description = "A major release",
                Release = ReleaseType.Major,
                Priority = 15
            },
            new()
            {
                Id = "breaking",
                Name = "Breaking",
                Description = "A breaking change",
                Release = ReleaseType.Major,
                Priority = 14
            },
            new()
            {
                Id = "minor",
                Name = "Minor",
                Description = "A minor release",
                Release = ReleaseType.Minor,
                Priority = 13
            },
            new()
            {
                Id = "feat",
                AlternativeIds = ["feature"],
                Name = "New Features",
                Description = "A new feature",
                Release = ReleaseType.Minor,
                Priority = 12
            },
            new()
            {
                Id = "patch",
                Name = "Patch",
                Description = "A patch release",
                Release = ReleaseType.Patch,
                Priority = 11
            },
            new()
            {
                Id = "fix",
                AlternativeIds = ["bug"],
                Name = "Bug Fixes",
                Description = "A bug fix",
                Release = ReleaseType.Patch,
                Priority = 10
            },
            new()
            {
                Id = "perf",
                Name = "Performance",
                Description = "A code change that improves performance",
                Release = ReleaseType.Patch,
                Priority = 9
            },
            new()
            {
                Id = "build",
                Name = "Build",
                Description = "Changes that affect the build system",
                Release = ReleaseType.Patch,
                Priority = 8
            },
            new()
            {
                Id = "deps",
                Name = "Dependencies",
                Description = "Changes to dependencies",
                Release = ReleaseType.Patch,
                Priority = 7
            },
            new()
            {
                Id = "docs",
                Name = "Documentation",
                Description = "Changes affecting the documentation, including xml-doc",
                Release = ReleaseType.Patch,
                Priority = 6
            },
            new()
            {
                Id = "revert",
                Name = "Revert",
                Description = "Reverting of previous changes",
                Release = ReleaseType.Patch,
                Priority = 5
            },
            new()
            {
                Id = "style",
                AlternativeIds = ["styles"],
                Name = "Code Style",
                Description = "Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)",
                Release = ReleaseType.None,
                Priority = 4
            },
            new()
            {
                Id = "refactor",
                Name = "Refactor",
                Description = "A code change that neither fixes a bug nor adds a feature",
                Release = ReleaseType.None,
                Priority = 3
            },
            new()
            {
                Id = "test",
                Name = "Tests",
                Description = "Adding, removing or revising tests",
                Release = ReleaseType.None,
                Priority = 2
            },
            new()
            {
                Id = "ci",
                Name = "CI",
                Description = "Changes to the CI pipeline",
                Release = ReleaseType.None,
                Priority = 1
            },
            new()
            {
                Id = "chore",
                Name = "Chores",
                Description = "Other changes that don't affect the meaning of the code",
                Release = ReleaseType.None,
                Priority = 0
            }

        };
    }
}
