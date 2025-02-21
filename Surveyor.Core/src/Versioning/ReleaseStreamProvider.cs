using System.Text.RegularExpressions;
using Surveyor.System;

namespace Surveyor.Versioning;

/// <summary>
/// A provider for <see cref="ReleaseStream"/>
/// </summary>
public interface IReleaseStreamProvider
{
    /// <summary>
    /// Get a <see cref="ReleaseStream"/> by the branch name.
    /// </summary>
    /// <param name="branchName">The name of the branch.</param>
    /// <returns>
    /// The <see cref="ReleaseStream"/> if found; otherwise, <see langword="null"/>.
    /// </returns>
    ReleaseStream? Get(string branchName);
}

/// <inheritdoc/>
public class ReleaseStreamProvider : IReleaseStreamProvider
{
    private readonly IReadOnlyCollection<ReleaseStream> _streams;

    /// <summary>
    /// Creates a new instance of <see cref="ReleaseStreamProvider"/>.
    /// </summary>
    public ReleaseStreamProvider()
    {
        _streams = DefaultTypes();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ReleaseStreamProvider"/>.
    /// </summary>
    public ReleaseStreamProvider(IReadOnlyCollection<ReleaseStream> streams)
    {
        _streams = streams;
    }

    /// <inheritdoc/>
    public ReleaseStream? Get(string branchName)
    {
        ReleaseStream? exactMatch = _streams
            .FirstOrNull(x => x.BranchName is not null && x.BranchName.Equals(branchName));
        if (exactMatch is ReleaseStream releaseStream)
            return releaseStream;
        return _streams
            .FirstOrNull(x =>
            {
                if (x.BranchNamePattern is null)
                    return false;
                Regex regex = new(x.BranchNamePattern);
                return regex.IsMatch(branchName);

            });
    }

    /// <summary>
    /// Get the default types.
    /// </summary>
    public IReadOnlyCollection<ReleaseStream> DefaultTypes()
    {
        return new ReleaseStream[]
        {
            new()
            {
                Id = "release",
                BranchName = "release",
                IsPrimary = true
            },
            new()
            {
                Id = "alpha",
                BranchName = "main",
                IsPreRelease = true
            },
            new()
            {
                Id = "alpha",
                BranchName = "alpha",
                IsPreRelease = true
            },
            new()
            {
                Id = "beta",
                BranchName = "beta",
                IsPreRelease = true
            },
            new()
            {
                Id = "rc",
                BranchName = "rc",
                IsPreRelease = true
            },
            new()
            {
                Id = "major",
                BranchNamePattern = "^v([1-9][0-9]*)$"
            },
            new()
            {
                Id = "minor",
                BranchNamePattern = "^v([1-9][0-9]*).([1-9][0-9]*)$"
            },
            new()
            {
                Id = "patch",
                BranchNamePattern = "^v([1-9][0-9]*).([1-9][0-9]*).([1-9][0-9]*)$"
            }
        };
    }
}
