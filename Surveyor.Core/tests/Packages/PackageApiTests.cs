using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Surveyor.Packages;
using Surveyor.Utils.Versioning;

namespace Surveyor.Core.Tests.Packages;

internal sealed class PackageApiTests
{
    private readonly PackageApi _api;

    public PackageApiTests()
    {
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddTransient<PackageApi>()
                .AddOptions<PackageApiOptions>()
                .BindConfiguration(PackageApiOptions.Section))
            .Build();
        _api = host.Services.GetRequiredService<PackageApi>();
    }

    [Test]
    public async Task PackageApi_GetPackageVersions([Values] bool includePrerelease)
    {
        // Arrange

        // Act
        IReadOnlyCollection<SemanticVersion> versions = await _api.GetPackageVersions("StudioLE.Example", includePrerelease);

        // Assert
        Assert.That(versions, Is.Not.Null);
        Assert.That(versions, Is.Not.Empty);
    }
}
