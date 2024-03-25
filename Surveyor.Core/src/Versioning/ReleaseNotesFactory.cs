using StudioLE.Extensions.System;
using StudioLE.Patterns;

namespace Surveyor.Versioning;

/// <summary>
/// Create release notes from a collection of conventional commits.
/// </summary>
public class ReleaseNotesByScopeFactory : IFactory<IReadOnlyCollection<ConventionalCommit>, string>
{
    private readonly ConventionalCommitTypeProvider _types;

    /// <summary>
    /// Creates a new instance of <see cref="ReleaseNotesByScopeFactory"/>.
    /// </summary>
    public ReleaseNotesByScopeFactory(ConventionalCommitTypeProvider types)
    {
        _types = types;
    }

    /// <summary>
    /// Create release notes from a collection of conventional commits.
    /// </summary>
    /// <param name="commits">The commits to use.</param>
    /// <returns>
    /// Release notes formatted as markdown.
    /// </returns>
    public string Create(IReadOnlyCollection<ConventionalCommit> commits)
    {
        string[] sections = commits
            .GroupBy(x => x.Scope)
            .Select(x => CreatePerScopeSections(x.Key, x.ToArray()))
            .ToArray();
        return string.Join(Environment.NewLine, sections);
    }

    private string CreatePerScopeSections(string scope, IReadOnlyCollection<ConventionalCommit> commits)
    {
        if (string.IsNullOrEmpty(scope))
            scope = "Global Improvements";
        string body = commits
            .OrderByDescending(x => x.Release)
            .GroupBy(x => x.TypeId)
            .Select(x => CreatePerTypeSections(x.Key, x.ToArray()))
            .Join();
        return $"""
                # {scope}

                {body}
                """;
    }

    private string CreatePerTypeSections(string type, IReadOnlyCollection<ConventionalCommit> commits)
    {
        string typeFriendlyName = _types.Get(type)?.Name ?? "Other Improvements";
        string body = commits
            .Select(Create)
            .Join();
        return $"""
                ### {typeFriendlyName}

                {body}
                """;
    }

    private static string Create(ConventionalCommit commit)
    {
        string summary = commit.IsBreaking
            ? $"Breaking Change: <strong>{commit.Subject}</strong>  {commit.Hash}"
            : $"<strong>{commit.Subject}</strong> {commit.Hash}";
        string details = string.IsNullOrWhiteSpace(commit.Body) && commit.Footers.Count == 0
            ? "No further information provided..."
            : $"""
               {commit.Body.Trim()}

               {FormatAsYaml(commit.Footers)}
               """;
        string alert = commit.IsBreaking ? "[!WARNING]" : string.Empty;
        string result = $"""
                         {alert}
                         {FormatAsCollapsable(summary, details)}
                         """;
        return FormatAsBlockQuote(result) + Environment.NewLine;
    }

    private static string FormatAsYaml(IReadOnlyDictionary<string, string> dictionary)
    {
        if (dictionary.Count == 0)
            return string.Empty;
        string yaml = dictionary.Select(x => $"{x.Key}: {x.Value}").Join();
        return $"""
                ```yaml
                {yaml}
                ```
                """;
    }

    private static string FormatAsBlockQuote(string str)
    {
        return str
            .Trim()
            .Split(['\r', '\n'])
            .Select(line => $"> {line}")
            .Join();
    }

    private static string FormatAsCollapsable(string summary, string details)
    {
        return $"""
                <details>
                <summary>{summary}</summary>
                <br>

                {details.Trim()}
                </details>
                """;
    }
}
