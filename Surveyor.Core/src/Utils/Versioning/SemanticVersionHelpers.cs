namespace Surveyor.Utils.Versioning;

/// <summary>
/// Methods to help with <see cref="SemanticVersion"/>.
/// </summary>
public static class SemanticVersionHelpers
{
    /// <summary>
    /// Create a new <see cref="SemanticVersion"/> with the major number bumped by one.
    /// </summary>
    /// <param name="current">The version to be increased.</param>
    /// <returns>
    /// A new <see cref="SemanticVersion"/> with the major number bumped by one.
    /// </returns>
    public static SemanticVersion BumpMajor(SemanticVersion current)
    {
        return new()
        {
            Major = current.Major + 1
        };
    }

    /// <summary>
    /// Create a new <see cref="SemanticVersion"/> with the minor number bumped by one.
    /// </summary>
    /// <param name="current">The version to be increased.</param>
    /// <returns>
    /// A new <see cref="SemanticVersion"/> with the minor number bumped by one.
    /// </returns>
    public static SemanticVersion BumpMinor(SemanticVersion current)
    {
        return new()
        {
            Major = current.Major,
            Minor = current.Minor + 1
        };
    }

    /// <summary>
    /// Create a new <see cref="SemanticVersion"/> with the patch number bumped by one.
    /// </summary>
    /// <param name="current">The version to be increased.</param>
    /// <returns>
    /// A new <see cref="SemanticVersion"/> with the patch number bumped by one.
    /// </returns>
    public static SemanticVersion BumpPatch(SemanticVersion current)
    {
        return new()
        {
            Major = current.Major,
            Minor = current.Minor,
            Patch = current.Patch + 1
        };
    }

    /// <summary>
    /// Determine if the version is a pre-release.
    /// </summary>
    /// <param name="version">The version to check.</param>
    /// <returns>
    /// <see langword="true"/> if the version is a pre-release; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsPrerelease(this SemanticVersion version)
    {
        return !string.IsNullOrEmpty(version.PreRelease);
    }
}
