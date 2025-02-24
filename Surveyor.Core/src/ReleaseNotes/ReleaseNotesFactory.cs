using Microsoft.Extensions.Options;
using StudioLE.Extensions.System;
using StudioLE.Patterns;
using Surveyor.VersionControl;

namespace Surveyor.ReleaseNotes;

/// <summary>
/// Create release notes from a collection of conventional commits.
/// </summary>
public interface IReleaseNotesFactory : IFactory<IReadOnlyCollection<ConventionalCommit>, string>
{
}

/// <summary>
/// Create release notes from a collection of conventional commits.
/// </summary>
public class ReleaseNotesFactory : IReleaseNotesFactory
{
    private readonly ReleaseNotesActivityOptions _options;
    private readonly ConventionalCommitTypeProvider _types;

    /// <summary>
    /// Creates a new instance of <see cref="ReleaseNotesFactory"/>.
    /// </summary>
    public ReleaseNotesFactory(IOptions<ReleaseNotesActivityOptions> options, ConventionalCommitTypeProvider types)
    {
        _options = options.Value;
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
            .GroupBy(x => _options.GroupByScope ? x.Scope : string.Empty)
            .OrderBy(x => x.Key)
            .Select(x => CreatePerScopeSections(x.Key, x.ToArray()))
            .ToArray();
        return string.Join(Environment.NewLine, sections);
    }

    private string CreatePerScopeSections(string scope, IReadOnlyCollection<ConventionalCommit> commits)
    {
        if (string.IsNullOrEmpty(scope))
            scope = string.IsNullOrEmpty(_options.DefaultSectionTitle)
                ? "Changes"
                : _options.DefaultSectionTitle;
        string body = commits
            .GroupBy(x => _types.Get(x.TypeId))
            .OrderByDescending(x => x.Key?.Priority ?? -1)
            .Select(x => CreatePerTypeSections(x.Key, x.ToArray()))
            .Join();
        return $"""
                ## {scope}

                {body}
                """;
    }

    private string CreatePerTypeSections(ConventionalCommitType? type, IReadOnlyCollection<ConventionalCommit> commits)
    {
        string typeFriendlyName = type?.Name ?? "Other";
        string body = commits
            .Select(Create)
            .Join();
        return $"""
                #### {typeFriendlyName}

                {body}
                """;
    }

    private string Create(ConventionalCommit commit)
    {
        string scope = _options.GroupByScope || string.IsNullOrEmpty(commit.Scope)
            ? string.Empty
            : $"{commit.Scope}: ";
        string summary = commit.IsBreaking
            ? $"Breaking Change: {scope}<strong>{commit.Subject}</strong>  {commit.Hash}"
            : $"{scope}<strong>{commit.Subject}</strong> {commit.Hash}";
        string? details = string.IsNullOrWhiteSpace(commit.Body) && commit.Footers.Count == 0
            ? null
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
            .Replace("\r\n", "\n")
            .Split('\n')
            .Select(line => $"> {line}")
            .Join();
    }

    private static string FormatAsCollapsable(string summary, string? details)
    {
        return string.IsNullOrEmpty(details)
            ? summary
            : $"""
               <details>
               <summary>{summary}</summary>
               <br>

               {details!.Trim()}
               </details>
               """;
    }
}
