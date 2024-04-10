using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Surveyor.Hosting;
using Surveyor.Projects;
using Surveyor.ReleaseNotes;
using Surveyor.Versioning;

namespace Surveyor.Cli;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging => logging
                .AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Debug)
                .SetMinimumLevel(LogLevel.Debug))
            .ConfigureServices((_, services) => services
                .AddSurveyorServices()
                .AddSurveyorOptions())
            .Build();
        string[] commands = args
            .TakeWhile(x => !x.StartsWith('-'))
            .ToArray();
        if (commands.Length == 0)
        {
            Console.Error.WriteLine("No command specified.");
            Environment.Exit(1);
        }
        Task task = commands.First() switch
        {
            "version" => ExecuteRepositoryVersioning(host.Services),
            "project-version" => ExecuteProjectVersioning(host.Services),
            "release-notes" => ExecuteReleaseNotes(host.Services),
            "readme" => ExecuteReadMe(host.Services),
            _ => InvalidCommand()
        };
        await task;
    }

    private static async Task ExecuteProjectVersioning(IServiceProvider services)
    {
        ProjectVersioningActivity activity = services.GetRequiredService<ProjectVersioningActivity>();
        IOptions<VersioningActivityOptions> options = services.GetRequiredService<IOptions<VersioningActivityOptions>>();
        SemanticVersion? versionQuery = await activity.Execute(options.Value);
        if (versionQuery is SemanticVersion version)
            Console.WriteLine(version.ToString());
        Environment.Exit(0);
    }

    private static Task ExecuteRepositoryVersioning(IServiceProvider services)
    {
        RepositoryVersioningActivity activity = services.GetRequiredService<RepositoryVersioningActivity>();
        IOptions<VersioningActivityOptions> options = services.GetRequiredService<IOptions<VersioningActivityOptions>>();
        SemanticVersion? versionQuery = activity.Execute(options.Value);
        if (versionQuery is SemanticVersion version)
            Console.WriteLine(version.ToString());
        Environment.Exit(0);
        return Task.CompletedTask;
    }

    private static Task ExecuteReleaseNotes(IServiceProvider services)
    {
        ReleaseNotesActivity activity = services.GetRequiredService<ReleaseNotesActivity>();
        IOptions<ReleaseNotesActivityOptions> options = services.GetRequiredService<IOptions<ReleaseNotesActivityOptions>>();
        string releaseNotes = activity.Execute(options.Value);
        if (!string.IsNullOrEmpty(releaseNotes))
        {
            Console.WriteLine(releaseNotes);
            Environment.Exit(0);
        }
        else
        {
            Console.Error.WriteLine("Failed to produce release notes.");
            Environment.Exit(1);
        }
        return Task.CompletedTask;
    }

    private static Task ExecuteReadMe(IServiceProvider services)
    {
        ReadMeActivity activity = services.GetRequiredService<ReadMeActivity>();
        string readme = activity.Execute();
        if (!string.IsNullOrEmpty(readme))
        {
            Console.WriteLine(readme);
            Environment.Exit(0);
        }
        else
        {
            Console.Error.WriteLine("Failed to produce README.");
            Environment.Exit(1);
        }
        return Task.CompletedTask;
    }

    private static Task InvalidCommand()
    {
        Console.Error.WriteLine("Invalid command.");
        Environment.Exit(1);
        return Task.CompletedTask;
    }
}
