using Surveyor.Versioning;

namespace Surveyor.VersionControl;

/// <summary>
/// A conventional commit.
/// </summary>
public readonly record struct ConventionalCommit()
{
    /// <summary>
    /// The hash.
    /// </summary>
    public string Hash { get; init; } = string.Empty;

    /// <summary>
    /// The optional id type.
    /// </summary>
    public string TypeId { get; init; } = string.Empty;

    /// <summary>
    /// The optional scope.
    /// </summary>
    public string Scope { get; init; } = string.Empty;

    /// <summary>
    /// Is this a breaking change?
    /// </summary>
    public bool IsBreaking { get; init; } = false;

    /// <summary>
    /// The subject.
    /// </summary>
    /// <remarks>
    /// The subject is the first line message after the scope.
    /// It is alternatively known as the description.
    /// </remarks>
    public string Subject { get; init; } = string.Empty;

    /// <summary>
    /// The optional body.
    /// </summary>
    /// <remarks>
    /// The body is the remainder message after the description.
    /// </remarks>
    public string Body { get; init; } = string.Empty;

    /// <summary>
    /// The optional footers.
    /// </summary>
    /// <remarks>
    /// The body is the remainder message after the description.
    /// </remarks>
    public IReadOnlyDictionary<string, string> Footers { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// The release type the conventional commit evaluates to.
    /// </summary>
    public ReleaseType Release { get; init; } = ReleaseType.None;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{TypeId}({Scope}): {Subject} ({Hash.Substring(0, 7)})";
    }
}
