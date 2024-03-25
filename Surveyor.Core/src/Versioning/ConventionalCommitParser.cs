using System.Text.RegularExpressions;

namespace Surveyor.Versioning;

/// <summary>
/// Parse git commit messages according to the Conventional Commits specification.
/// </summary>
public class ConventionalCommitParser
{
    private readonly ConventionalCommitTypeProvider _types;

    /// <summary>
    /// Creates a new instance of <see cref="ConventionalCommitParser"/>.
    /// </summary>
    public ConventionalCommitParser(ConventionalCommitTypeProvider types)
    {
        _types = types;
    }

    /// <summary>
    /// Create a <see cref="ConventionalCommit"/> by parsing the given commit message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>
    /// A <see cref="ConventionalCommit"/> if the message is a conventional commit; otherwise, <see langword="null"/>.
    /// </returns>
    /// <seealso href="https://regex101.com/r/WxQNBj/1">Regex101</seealso>
    public ConventionalCommit? Parse(string message)
    {
        Regex regex = new(@"^(?:(\w+)(?:\(([^\r\n]+)\))?(!?):)[\t ]*([^\r\n]+)(?:\s+([\s\S]+))?$", RegexOptions.Singleline);
        Match match = regex.Match(message);
        if (!match.Success)
            return null;
        string type = match.Groups[1].Value;
        string scope = match.Groups[2].Value;
        bool isBreaking = match.Groups[3].Value == "!";
        string subject = match.Groups[4].Value;
        string body = match.Groups[5].Value;
        string[] bodyLines = body.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        if (bodyLines.Any(x => x.StartsWith("BREAKING CHANGE:")))
            isBreaking = true;
        body = string.Empty;
        Dictionary<string, string> footers = new();
        foreach (string line in bodyLines.Reverse())
        {
            Regex trailerRegex = new(@"^([a-zA-Z0-9_-]+):[ \t]*(.+)$");
            Match trailerMatch = trailerRegex.Match(line);
            if (trailerMatch.Success)
            {
                string key = trailerMatch.Groups[1].Value;
                string value = trailerMatch.Groups[2].Value;
                if (!string.IsNullOrEmpty(body))
                {
                    value += Environment.NewLine + body;
                    body = string.Empty;
                }
                footers[key] = value;
            }
            else
                body = line + Environment.NewLine + body;
        }
        return new ConventionalCommit
        {
            TypeId = type,
            IsBreaking = isBreaking,
            Scope = scope,
            Subject = subject,
            Body = body,
            Footers = footers,
            Release = isBreaking
                ? ReleaseType.Major
                : GetReleaseType(type)
        };
    }

    private ReleaseType GetReleaseType(string id)
    {
        ConventionalCommitType? typeQuery = _types.Get(id);
        if (typeQuery is ConventionalCommitType type)
            return type.Release;
        return ReleaseType.Unknown;
    }
}
