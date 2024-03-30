using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Surveyor.Hosting;
using Surveyor.Versioning;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => services
        .AddSurveyorServices()
        .AddSurveyorOptions())
    .Build();

VersioningActivity activity = host.Services.GetRequiredService<VersioningActivity>();
IOptions<VersioningActivityOptions> options = host.Services.GetRequiredService<IOptions<VersioningActivityOptions>>();
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
