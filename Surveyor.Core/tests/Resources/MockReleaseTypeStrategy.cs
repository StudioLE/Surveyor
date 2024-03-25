using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Resources;

public class MockReleaseTypeStrategy : IReleaseTypeStrategy
{
    private readonly ReleaseType _releaseType;

    public MockReleaseTypeStrategy(ReleaseType releaseType)
    {
        _releaseType = releaseType;
    }

    /// <inheritdoc />
    public ReleaseType Get()
    {
        return _releaseType;
    }

    /// <inheritdoc/>
    public ReleaseType Get(SemanticVersion latestVersion)
    {
        return _releaseType;
    }
}
