using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Resources;

internal class MockHeadVersionProvider : IHeadVersionProvider
{
    private readonly SemanticVersion[] _versions;

    public MockHeadVersionProvider(SemanticVersion[] versions)
    {
        _versions = versions;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<SemanticVersion> Get(string branchName)
    {
        return _versions;
    }
}
