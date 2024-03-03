using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Surveyor.Packages;

namespace Surveyor.Core.Tests.Packages;

internal sealed class PackagesApiTests
{
    private readonly PackagesApi _api;

    public PackagesApiTests()
    {
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddTransient<PackagesApi>()
                .AddOptions<PackagesApiOptions>()
                .BindConfiguration(PackagesApiOptions.PackagesSection))
            .Build();
        _api = host.Services.GetRequiredService<PackagesApi>();
    }

    [Test]
    public async Task PackagesApi_GetPackageVersions([Values] bool includePrerelease)
    {
        // Arrange

        // Act
        IReadOnlyCollection<string> versions = await _api.GetPackageVersions("StudioLE.Example", includePrerelease);

        // Assert
        Assert.That(versions, Is.Not.Null);
        Assert.That(versions, Is.Not.Empty);
    }
}
