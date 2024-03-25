using Surveyor.Versioning;

namespace Surveyor.Core.Tests.Resources;

internal class MockChangedFileProvider : IChangedFileProvider
{
    private readonly string[] _files;

    public MockChangedFileProvider(string[] files)
    {
        _files = files;
    }

    /// <inheritdoc />
    public IEnumerable<string> Get(string projectDirectory)
    {
        return _files;
    }

    /// <inheritdoc />
    public IEnumerable<string> Get(string projectDirectory, SemanticVersion version)
    {
        return _files;
    }
}
