using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Surveyor.Build;
using Surveyor.Packages;

namespace Surveyor.Core.Tests.Build;

internal sealed class LatestVersionTests
{
    [Test]
    public void LatestVersion_Execute()
    {
        // Arrange
        LatestVersion task = new()
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
    public void LatestVersion_Execute_WithAuthToken()
    {
        // Arrange
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddTransient<PackageApi>()
                .AddOptions<PackageApiOptions>()
                .BindConfiguration(PackageApiOptions.Section))
            .Build();
        IOptions<PackageApiOptions> options = host.Services.GetRequiredService<IOptions<PackageApiOptions>>();
        LatestVersion task = new()
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
