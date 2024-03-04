using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Surveyor.Build;
using Surveyor.Packages;

namespace Surveyor.Core.Tests.Build;

internal sealed class LatestVersionTests
{
    private readonly IOptions<PackageApiOptions> _options;

    public LatestVersionTests()
    {
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddTransient<PackageApi>()
                .AddOptions<PackageApiOptions>()
                .BindConfiguration(PackageApiOptions.Section))
            .Build();
        _options = host.Services.GetRequiredService<IOptions<PackageApiOptions>>();
    }

    [Test]
    public void LatestVersion_Execute()
    {
        // Arrange
        LatestVersion task = new()
        {
            PackageName = "StudioLE.Example",
            Feed = _options.Value.Feed,
            AuthToken = _options.Value.AuthToken
        };

        // Act
        bool result = task.Execute();

        // Assert
        Assert.That(result);
        Assert.That(task.Latest, Is.Not.Empty);
    }
}
