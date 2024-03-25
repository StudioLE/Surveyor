using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Resources;

internal class MockBranchVersionProvider : IBranchVersionProvider
{
    private readonly SemanticVersion[] _versions;

    public MockBranchVersionProvider(SemanticVersion[] versions)
    {
        _versions = versions;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<SemanticVersion> Get(string branchName)
    {
        return _versions;
    }
}
