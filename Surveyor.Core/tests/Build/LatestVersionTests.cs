using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Surveyor.Build;
using Surveyor.Packages;

namespace Surveyor.Core.Tests.Build;

internal sealed class LatestVersionTests
{
    private readonly IOptions<PackagesApiOptions> _options;

    public LatestVersionTests()
    {
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddTransient<PackagesApi>()
                .AddOptions<PackagesApiOptions>()
                .BindConfiguration(PackagesApiOptions.PackagesSection))
            .Build();
        _options = host.Services.GetRequiredService<IOptions<PackagesApiOptions>>();
    }

    [Test]
    public void LatestVersion_Execute()
    {
        // Arrange
        LatestVersion task = new()
        {
            PackageName = "StudioLE.Example",
            PackageFeed = _options.Value.BaseAddress,
            AuthToken = _options.Value.AuthToken
        };

        // Act
        bool result = task.Execute();

        // Assert
        Assert.That(result);
        Assert.That(task.Latest, Is.Not.Empty);
    }
}
