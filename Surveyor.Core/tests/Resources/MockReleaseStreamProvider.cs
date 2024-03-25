using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Resources;

public class MockReleaseStreamProvider : IReleaseStreamProvider
{
    private readonly ReleaseStream? _releaseStream;

    public MockReleaseStreamProvider(ReleaseStream? releaseStream)
    {
        _releaseStream = releaseStream;
    }

    /// <inheritdoc />
    public ReleaseStream? Get(string branchName)
    {
        return _releaseStream;
    }
}
