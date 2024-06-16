using Microsoft.Extensions.DependencyInjection;
using Surveyor.Packages;
using Surveyor.Projects;
using Surveyor.ReleaseNotes;
using Surveyor.VersionControl;
using Surveyor.Versioning;

namespace Surveyor.Hosting;

/// <summary>
/// Methods to help with <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Add the services required by Surveyor for dependency injection.
    /// </summary>
    public static IServiceCollection AddSurveyorServices(this IServiceCollection services)
    {
        return services
            .AddTransient<ProjectVersioningActivity>()
            .AddTransient<RepositoryVersioningActivity>()
            .AddTransient<IBranchVersionProvider, BranchVersionProvider>()
            .AddTransient<IChangedFileProvider, ChangedFileProvider>()
            .AddTransient<IPublishedVersionProvider, PublishedVersionProvider>()
            .AddTransient<IRepositoryVersionProvider, RepositoryVersionProvider>()
            .AddTransient<IHeadVersionProvider, HeadVersionProvider>()
            .AddTransient<IReleaseTypeStrategy, ReleaseTypeStrategy>()
            .AddTransient<PackageApi>()
            .AddSingleton<GitCli>()
            .AddTransient<IReleaseStreamProvider, ReleaseStreamProvider>()
            .AddTransient<ReleaseNotesActivity>()
            .AddTransient<ConventionalCommitTypeProvider>()
            .AddTransient<IReleaseNotesFactory, ReleaseNotesFactory>()
            .AddTransient<ReadMeActivity>();
    }

    /// <summary>
    /// Add the options required by Surveyor for dependency injection.
    /// </summary>
    public static IServiceCollection AddSurveyorOptions(this IServiceCollection services)
    {
        services
            .AddOptions<PackageApiOptions>()
            .BindConfiguration(PackageApiOptions.Section);
        services
            .AddOptions<GitCliOptions>()
            .BindConfiguration(GitCliOptions.Section);
        services
            .AddOptions<VersioningActivityOptions>()
            .BindConfiguration(VersioningActivityOptions.Section);
        return services;
    }
}
