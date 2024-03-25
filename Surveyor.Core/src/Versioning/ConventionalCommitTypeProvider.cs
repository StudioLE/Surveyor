using Surveyor.System;

namespace Surveyor.Versioning;

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
                Name = "Major Improvements",
                Description = "A major release",
                Release = ReleaseType.Major
            },
            new()
            {
                Id = "breaking",
                Name = "Breaking Changes",
                Description = "A breaking change",
                Release = ReleaseType.Major
            },
            new()
            {
                Id = "minor",
                Name = "Minor Improvements",
                Description = "A minor release",
                Release = ReleaseType.Minor
            },
            new()
            {
                Id = "feat",
                AlternativeIds = ["feature"],
                Name = "New Features",
                Description = "A new feature",
                Release = ReleaseType.Minor
            },
            new()
            {
                Id = "patch",
                Name = "Patch Improvements",
                Description = "A patch release",
                Release = ReleaseType.Patch
            },
            new()
            {
                Id = "fix",
                AlternativeIds = ["bug"],
                Name = "Bug Fixes",
                Description = "A bug fix",
                Release = ReleaseType.Patch
            },
            new()
            {
                Id = "perf",
                Name = "Performance Improvements",
                Description = "A code change that improves performance",
                Release = ReleaseType.Patch
            },
            new()
            {
                Id = "build",
                Name = "Build Improvements",
                Description = "Changes that affect the build system",
                Release = ReleaseType.Patch
            },
            new()
            {
                Id = "deps",
                Name = "Dependency Improvements",
                Description = "Changes to dependencies",
                Release = ReleaseType.Patch
            },
            new()
            {
                Id = "docs",
                Name = "Documentation Improvements",
                Description = "Changes affecting the documentation, including xml-doc",
                Release = ReleaseType.Patch
            },
            new()
            {
                Id = "revert",
                Name = "Reversions",
                Description = "Reverting of previous changes",
                Release = ReleaseType.Patch
            },
            new()
            {
                Id = "style",
                AlternativeIds = ["styles"],
                Name = "Style Improvements",
                Description = "Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)",
                Release = ReleaseType.None
            },
            new()
            {
                Id = "refactor",
                Name = "Refactors",
                Description = "A code change that neither fixes a bug nor adds a feature",
                Release = ReleaseType.None
            },
            new()
            {
                Id = "test",
                Name = "Test Improvements",
                Description = "Adding, removing or revising tests",
                Release = ReleaseType.None
            },
            new()
            {
                Id = "ci",
                Name = "Continuous Integration Improvements",
                Description = "Changes to the CI pipeline",
                Release = ReleaseType.None
            },
            new()
            {
                Id = "chore",
                Name = "Chores",
                Description = "Other changes that don't affect the meaning of the code",
                Release = ReleaseType.None
            }

        };
    }
}
