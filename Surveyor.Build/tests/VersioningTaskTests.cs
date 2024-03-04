using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Surveyor.Packages;

namespace Surveyor.Build.Tests;

internal sealed class VersioningTaskTests
{
    [Test]
    public void VersioningTask_Execute()
    {
        // Arrange
        VersioningTask task = new()
        {
            PackageName = "Microsoft.Extensions.Logging.Abstractions"
        };

        // Act
        bool result = task.Execute();

        // Assert
        Assert.That(result);
        Assert.That(task.Latest, Is.Not.Empty);
    }

    [Test]
    [Explicit("Requires auth token")]
    public void VersioningTask_Execute_WithAuthToken()
    {
        // Arrange
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddTransient<PackageApi>()
                .AddOptions<PackageApiOptions>()
                .BindConfiguration(PackageApiOptions.Section))
            .Build();
        IOptions<PackageApiOptions> options = host.Services.GetRequiredService<IOptions<PackageApiOptions>>();
        VersioningTask task = new()
        {
            PackageName = "StudioLE.Example",
            Feed = options.Value.Feed,
            AuthToken = options.Value.AuthToken
        };

        // Act
        bool result = task.Execute();

        // Assert
        Assert.That(result);
        Assert.That(task.Latest, Is.Not.Empty);
    }
}
