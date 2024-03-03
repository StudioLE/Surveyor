using System.Text;
using System.Text.RegularExpressions;

namespace Surveyor.Utils.Versioning;

/// <summary>
/// A version number following the Semantic Versioning 2.0.0 specification.
/// </summary>
/// <remarks>
/// MAJOR.MINOR.PATCH-PRERELEASE.BUILD
/// </remarks>
/// <example>
/// 1.0.0
/// </example>
/// <example>
/// 1.0.0-alpha
/// </example>
/// <example>
/// 1.0.0-alpha.1
/// </example>
/// <seealso href="https://semver.org/spec/v2.0.0.html"/>
public readonly record struct SemanticVersion()
{
    /// <seealso href="https://semver.org/spec/v2.0.0.html#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string"/>
    private const string Pattern = "^(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)(?:-((?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+([0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?$";

    /// <summary>
    /// The major component of the version.
    /// </summary>
    public int Major { get; init; } = 0;

    /// <summary>
    /// The minor component of the version.
    /// </summary>
    public int Minor { get; init; } = 0;

    /// <summary>
    /// The patch component of the version.
    /// </summary>
    public int Patch { get; init; } = 0;

    /// <summary>
    /// The optional pre-release component of the version.
    /// </summary>
    public string PreRelease { get; init; } = string.Empty;

    /// <summary>
    /// The optional build component of the version.
    /// </summary>
    public string Build { get; init; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append($"{Major}.{Minor}.{Patch}");
        if (!string.IsNullOrEmpty(PreRelease))
            builder.Append($"-{PreRelease}");
        if (!string.IsNullOrEmpty(Build))
            builder.Append($"+{Build}");
        return builder.ToString();
    }

    /// <summary>
    /// Create a <see cref="SemanticVersion"/> from a string.
    /// </summary>
    /// <param name="str">The version string.</param>
    /// <returns>
    /// The <see cref="SemanticVersion"/>, or <see langword="null"/> if parsing fails.
    /// </returns>
    public static SemanticVersion? Create(string str)
    {
        Match match = Regex.Match(str, Pattern);
        if (!match.Success)
            return null;
        return new SemanticVersion
        {
            Major = int.Parse(match.Groups[1].Value),
            Minor = int.Parse(match.Groups[2].Value),
            Patch = int.Parse(match.Groups[3].Value),
            PreRelease = match.Groups[4].Value,
            Build = match.Groups[5].Value
        };
    }
}
