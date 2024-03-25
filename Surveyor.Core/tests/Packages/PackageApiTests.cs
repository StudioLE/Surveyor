using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveyor.Packages;
using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Packages;

internal sealed class PackageApiTests
{
    [Test]
    public async Task PackageApi_GetPackageVersions()
    {
        // Arrange
        const string packageName = "Microsoft.Extensions.Logging.Abstractions";
        PackageApi api = new(new NullLogger<PackageApi>(), new PackageApiOptions());

        // Act
        IReadOnlyCollection<SemanticVersion> includingPrerelease = await api.GetPackageVersions(packageName, true);
        IReadOnlyCollection<SemanticVersion> excludingPrerelease = await api.GetPackageVersions(packageName, false);

        // Assert
        Assert.That(includingPrerelease, Is.Not.Null);
        Assert.That(includingPrerelease, Is.Not.Empty);
        Assert.That(excludingPrerelease, Is.Not.Null);
        Assert.That(excludingPrerelease, Is.Not.Empty);
        Assert.That(includingPrerelease.Count, Is.GreaterThan(excludingPrerelease.Count));
    }

    [Test]
    [Explicit("Requires authentication token")]
    public async Task PackageApi_GetPackageVersions_WithAuthToken()
    {
        // Arrange
        const string packageName = "StudioLE.Example";

        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddTransient<PackageApi>()
                .AddOptions<PackageApiOptions>()
                .BindConfiguration(PackageApiOptions.Section))
            .Build();
        PackageApi api = host.Services.GetRequiredService<PackageApi>();

        // Act
        IReadOnlyCollection<SemanticVersion> includingPrerelease = await api.GetPackageVersions(packageName, true);
        IReadOnlyCollection<SemanticVersion> excludingPrerelease = await api.GetPackageVersions(packageName, false);

        // Assert
        Assert.That(includingPrerelease, Is.Not.Null);
        Assert.That(includingPrerelease, Is.Not.Empty);
        Assert.That(excludingPrerelease, Is.Not.Null);
        Assert.That(excludingPrerelease, Is.Not.Empty);
        Assert.That(includingPrerelease.Count, Is.GreaterThan(excludingPrerelease.Count));
    }
}
