using System.Collections;
using Microsoft.Build.Framework;

namespace Surveyor.BuildVersioning.Tests.Resources;

public class MockBuildEngine : IBuildEngine
{
    public List<BuildErrorEventArgs> Errors { get; } = new();
    public List<BuildWarningEventArgs> Warnings { get; } = new();
    public List<BuildMessageEventArgs> Messages { get; } = new();
    public List<CustomBuildEventArgs> CustomEvents { get; } = new();

    /// <inheritdoc/>
    public bool ContinueOnError { get; }

    /// <inheritdoc/>
    public int LineNumberOfTaskNode { get; }

    /// <inheritdoc/>
    public int ColumnNumberOfTaskNode { get; }

    /// <inheritdoc/>
    public string ProjectFileOfTaskNode { get; } = string.Empty;

    /// <inheritdoc/>
    public void LogErrorEvent(BuildErrorEventArgs e)
    {
        Errors.Add(e);
    }

    /// <inheritdoc/>
    public void LogWarningEvent(BuildWarningEventArgs e)
    {
        Warnings.Add(e);
    }

    /// <inheritdoc/>
    public void LogMessageEvent(BuildMessageEventArgs e)
    {
        Messages.Add(e);
    }

    /// <inheritdoc/>
    public void LogCustomEvent(CustomBuildEventArgs e)
    {
        CustomEvents.Add(e);
    }

    /// <inheritdoc/>
    public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
    {
        throw new NotImplementedException();
    }
}
