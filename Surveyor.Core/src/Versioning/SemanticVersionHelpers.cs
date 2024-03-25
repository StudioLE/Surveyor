namespace Surveyor.Versioning;

/// <summary>
/// Methods to help with <see cref="SemanticVersion"/>.
/// </summary>
public static class SemanticVersionHelpers
{
    /// <summary>
    /// Create a new <see cref="SemanticVersion"/> with the <paramref name="releaseType"/> number bumped by one.
    /// Unless the version is a pre-release in which case the pre-release is removed if the release type is suitable.
    /// </summary>
    /// <remarks>
    /// <see cref="ReleaseType.None"/> and <see cref="ReleaseType.Unknown"/> will be bumped as if they were <see cref="ReleaseType.Patch"/>.
    /// </remarks>
    /// <param name="version">The version to be increased.</param>
    /// <param name="releaseType">The type of the release.</param>
    /// <returns>
    /// A new <see cref="SemanticVersion"/> with the appropriate number bumped.
    /// </returns>
    public static SemanticVersion Bump(SemanticVersion version, ReleaseType releaseType)
    {
        return releaseType switch
        {
            ReleaseType.Major => BumpMajor(version),
            ReleaseType.Minor => BumpMinor(version),
            ReleaseType.Patch => BumpPatch(version),
            _ => BumpPatch(version)
        };
    }

    /// <summary>
    /// Create a new <see cref="SemanticVersion"/> with the major number bumped by one.
    /// Unless the version is a pre-release in which case the pre-release is removed if the release type is suitable.
    /// </summary>
    /// <param name="version">The version to be increased.</param>
    /// <returns>
    /// A new <see cref="SemanticVersion"/> with the major number bumped by one.
    /// </returns>
    public static SemanticVersion BumpMajor(SemanticVersion version)
    {
        if (version.IsPreRelease())
        {
            ReleaseType releaseType = version.GetReleaseType();
            if (releaseType == ReleaseType.Major)
                return version with
                {
                    PreRelease = string.Empty
                };
        }
        return version with
        {
            Major = version.Major + 1,
            Minor = 0,
            Patch = 0,
            PreRelease = string.Empty
        };
    }

    /// <summary>
    /// Create a new <see cref="SemanticVersion"/> with the minor number bumped by one.
    /// Unless the version is a pre-release in which case the pre-release is removed if the release type is suitable.
    /// </summary>
    /// <param name="version">The version to be increased.</param>
    /// <returns>
    /// A new <see cref="SemanticVersion"/> with the minor number bumped by one.
    /// </returns>
    public static SemanticVersion BumpMinor(SemanticVersion version)
    {
        if (version.IsPreRelease())
        {
            ReleaseType releaseType = version.GetReleaseType();
            if (releaseType is ReleaseType.Minor or ReleaseType.Major)
                return version with
                {
                    PreRelease = string.Empty
                };
        }
        return version with
        {
            Minor = version.Minor + 1,
            Patch = 0,
            PreRelease = string.Empty
        };
    }

    /// <summary>
    /// Create a new <see cref="SemanticVersion"/> with the patch number bumped by one.
    /// Unless the version is a pre-release in which case the pre-release is removed if the release type is suitable.
    /// </summary>
    /// <param name="version">The version to be increased.</param>
    /// <returns>
    /// A new <see cref="SemanticVersion"/> with the patch number bumped by one.
    /// </returns>
    public static SemanticVersion BumpPatch(SemanticVersion version)
    {
        if (version.IsPreRelease())
        {
            ReleaseType releaseType = version.GetReleaseType();
            if (releaseType is ReleaseType.Patch or ReleaseType.Minor or ReleaseType.Major)
                return version with
                {
                    PreRelease = string.Empty
                };
        }
        return version with
        {
            Patch = version.Patch + 1,
            PreRelease = string.Empty
        };
    }

    /// <summary>
    /// Create a new <see cref="SemanticVersion"/> with the pre-release number bumped by one
    /// if the pre-release id is <paramref name="preReleaseId"/>.
    /// Otherwise create a new pre-release with the given <paramref name="preReleaseId"/>.
    /// </summary>
    /// <param name="version">The version to be increased.</param>
    /// <param name="preReleaseId">An optional pre-release id to prepend.</param>
    /// <returns>
    /// A new <see cref="SemanticVersion"/> with the patch number bumped by one.
    /// </returns>
    public static SemanticVersion BumpPreRelease(SemanticVersion version, string? preReleaseId)
    {
        if (string.IsNullOrEmpty(version.PreRelease))
            return CreatePreRelease(version, preReleaseId);
        string[] components = version.PreRelease.Split('.');
        if (components.First() != preReleaseId)
            return CreatePreRelease(version, preReleaseId);
        bool isComplete = false;
        string[] updatedComponents = components
            .Reverse()
            .Select(str =>
            {
                if (isComplete)
                    return str;
                if (!int.TryParse(str, out int number))
                    return str;
                isComplete = true;
                return (number + 1).ToString();
            })
            .Reverse()
            .ToArray();
        if (!isComplete)
            updatedComponents = updatedComponents.Append("1").ToArray();
        string preRelease = string.Join(".", updatedComponents);
        return version with { PreRelease = preRelease };
    }

    private static SemanticVersion CreatePreRelease(SemanticVersion version, string? preReleaseId)
    {
        return string.IsNullOrEmpty(preReleaseId)
            ? version with { PreRelease = "1" }
            : version with { PreRelease = $"{preReleaseId}.1" };
    }

    /// <summary>
    /// Determine if the version is a pre-release.
    /// </summary>
    /// <param name="version">The version to check.</param>
    /// <returns>
    /// <see langword="true"/> if the version is a pre-release; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsPreRelease(this SemanticVersion version)
    {
        return !string.IsNullOrEmpty(version.PreRelease);
    }

    /// <summary>
    /// Determine the release type of the version.
    /// </summary>
    /// <param name="version">The version to check.</param>
    /// <returns>The <see cref="ReleaseType"/> of the version.</returns>
    public static ReleaseType GetReleaseType(this SemanticVersion version)
    {
        return version switch
        {
            { Minor: 0, Patch: 0 } => ReleaseType.Major,
            { Patch: 0 } => ReleaseType.Minor,
            _ => ReleaseType.Patch
        };
    }
}
