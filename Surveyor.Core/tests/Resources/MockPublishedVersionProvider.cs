using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Resources;

internal class MockPublishedVersionProvider : IPublishedVersionProvider
{
    private readonly SemanticVersion[] _versions;

    public MockPublishedVersionProvider(SemanticVersion[] versions)
    {
        _versions = versions;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyCollection<SemanticVersion>> Get(string packageName)
    {
        return Task.FromResult<IReadOnlyCollection<SemanticVersion>>(_versions);
    }
}
