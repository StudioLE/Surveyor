using System.Text;
using System.Text.RegularExpressions;

namespace Surveyor.Versioning;

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
public readonly record struct SemanticVersion() : IComparable<SemanticVersion>
{
    /// <seealso href="https://semver.org/spec/v2.0.0.html#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string"/>
    private const string Pattern = "^v?(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)(?:-((?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+([0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?$";

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

    /// <inheritdoc />
    public int CompareTo(SemanticVersion other)
    {
        int majorComparison = Major.CompareTo(other.Major);
        if (majorComparison != 0)
            return majorComparison;
        int minorComparison = Minor.CompareTo(other.Minor);
        if (minorComparison != 0)
            return minorComparison;
        int patchComparison = Patch.CompareTo(other.Patch);
        if (patchComparison != 0)
            return patchComparison;
        int preReleaseComparison = Compare(PreRelease, other.PreRelease);
        if (preReleaseComparison != 0)
            return preReleaseComparison;
        return Compare(Build, other.Build);
    }

    private static int Compare(string left, string right)
    {
        if (left == right)
            return 0;
        if (left == string.Empty)
            return 1;
        if (right == string.Empty)
            return -1;
        // TODO: Split the string by . and compare each part
        string[] leftComponents = left.Split('.');
        string[] rightComponents = right.Split('.');
        for (int i = 0; i < leftComponents.Length; i++)
        {
            // If the left side is longer then it is greater
            if (i >= rightComponents.Length)
                return 1;
            int comparison = CompareStringAsInt(leftComponents[i], rightComponents[i]);
            if (comparison != 0)
                return comparison;
        }
        return -1;
    }

    private static int CompareStringAsInt(string? left, string? right)
    {
        if(int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightInt))
            return leftInt.CompareTo(rightInt);
        return string.Compare(left, right, StringComparison.InvariantCulture);
    }
}
