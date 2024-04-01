using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Surveyor.Hosting;
using Surveyor.Versioning;

namespace Surveyor.Cli;

public class Program
{

    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) => services
                .AddSurveyorServices()
                .AddSurveyorOptions())
            .Build();
        string[] commands = args
            .TakeWhile(x => !x.StartsWith('-'))
            .ToArray();
        if(commands.Length == 0)
        {
            Console.Error.WriteLine("No command specified.");
            Environment.Exit(1);
        }
        Task task = commands.First() switch
        {
            "version" => ExecuteVersioning(host.Services),
            _ => InvalidCommand()
        };
        await task;
    }

    private static async Task ExecuteVersioning(IServiceProvider services)
    {
        VersioningActivity activity = services.GetRequiredService<VersioningActivity>();
        IOptions<VersioningActivityOptions> options = services.GetRequiredService<IOptions<VersioningActivityOptions>>();
        SemanticVersion? versionQuery = await activity.Execute(options.Value);
        if (versionQuery is SemanticVersion version)
        {
            Console.WriteLine(version.ToString());
            Environment.Exit(0);
        }
        else
        {
            Console.Error.WriteLine("Failed to determine the latest version of the package.");
            Environment.Exit(1);
        }
    }

    private static Task InvalidCommand()
    {
        Console.Error.WriteLine("Invalid command.");
        Environment.Exit(1);
        return Task.CompletedTask;
    }
}
