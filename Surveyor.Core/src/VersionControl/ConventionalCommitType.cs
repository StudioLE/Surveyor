using Surveyor.Versioning;

namespace Surveyor.VersionControl;

/// <summary>
/// A type of conventional commit.
/// </summary>
public readonly struct ConventionalCommitType() : IEquatable<ConventionalCommitType>
{
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Alternative identifiers that the type matches with.
    /// </summary>
    /// <remarks>
    /// This allows for misspellings or synonyms to be used.
    /// </remarks>
    public string[] AlternativeIds { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The name or title.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// The semantic release type.
    /// </summary>
    public ReleaseType Release { get; init; } = ReleaseType.None;

    /// <summary>
    /// The priority to order by.
    /// </summary>
    public int Priority { get; init; } = 0;

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }

    /// <inheritdoc />
    public bool Equals(ConventionalCommitType other)
    {
        return Id == other.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
