namespace Surveyor.Versioning;

/// <summary>
/// The release type of a <see cref="SemanticVersion"/>.
/// </summary>
public enum ReleaseType
{
    /// <summary>
    /// An unknown release type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Not a release.
    /// </summary>
    None,

    /// <summary>
    /// A patch release.
    /// </summary>
    /// <example>0.0.1</example>
    Patch,

    /// <summary>
    /// A minor release.
    /// </summary>
    /// <example>0.1.0</example>
    Minor,

    /// <summary>
    /// A major release.
    /// </summary>
    /// <example>1.0.0</example>
    Major
}
