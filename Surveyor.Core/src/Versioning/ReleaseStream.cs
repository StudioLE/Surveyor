namespace Surveyor.Versioning;

/// <summary>
/// A release stream.
/// </summary>
public readonly struct ReleaseStream()
{
    /// <summary>
    /// The identifier used for pre-release.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// An optional string that exactly matches the branch name.
    /// </summary>
    public string? BranchName { get; init; } = null;

    /// <summary>
    /// An optional regular expression pattern that matches the branch name.
    /// </summary>
    public string? BranchNamePattern { get; init; } = null;

    /// <summary>
    /// Is this the primary release branch?
    /// </summary>
    public bool IsPrimary { get; init; } = false;

    /// <summary>
    /// Is this a pre-release branch?
    /// </summary>
    public bool IsPreRelease { get; init; } = false;
}
