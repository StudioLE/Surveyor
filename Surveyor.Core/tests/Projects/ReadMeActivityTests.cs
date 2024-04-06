using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Surveyor.Hosting;
using Surveyor.Projects;

namespace Surveyor.Core.Tests.Projects;

internal sealed class ReadMeActivityTests
{
    private readonly IServiceProvider _services;

    public ReadMeActivityTests()
    {
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddSurveyorServices()
                .AddSurveyorOptions())
            .Build();
        _services = host.Services;
    }

    [Test]
    [Explicit("Requires configuration")]
    public void ReadMeActivity_Execute_DI()
    {
        // Arrange
        ReadMeActivity activity = _services.GetRequiredService<ReadMeActivity>();

        // Act
        string notes = activity.Execute();

        // Assert
        Assert.That(notes, Is.Not.Empty);
    }
}
