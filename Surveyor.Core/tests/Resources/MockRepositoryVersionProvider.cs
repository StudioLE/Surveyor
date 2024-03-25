using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Resources;

public class MockRepositoryVersionProvider : IRepositoryVersionProvider
{
    private readonly SemanticVersion[] _versions;

    public MockRepositoryVersionProvider(SemanticVersion[] versions)
    {
        _versions = versions;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<SemanticVersion> Get()
    {
        return _versions;
    }
}
