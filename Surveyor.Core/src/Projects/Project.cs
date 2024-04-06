using System.Xml.Linq;

namespace Surveyor.Projects;

/// <summary>
/// A facade for accessing the values of a <c>.csproj</c> file.
/// </summary>
public class Project
{
    private readonly XDocument _xDocument;

    /// <summary>
    /// The file path of the project relative to the repository root.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// The source directory of the project relative to the repository root.
    /// </summary>
    public string? SourceDirectory { get; }

    /// <summary>
    /// Creates a new instance of <see cref="Project"/>.
    /// </summary>
    public Project(string relativePath, string absolutePath)
    {
        FilePath = relativePath;
        SourceDirectory = Path.GetDirectoryName(relativePath);
        _xDocument = XDocument.Load(absolutePath);
    }

    /// <summary>
    /// Get the description.
    /// </summary>
    /// <remarks>
    /// A long description for the assembly. If PackageDescription is not specified, then this property is also used as the description of the package.
    /// </remarks>
    /// <returns>The description, or <see langword="null"/> if not set.</returns>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target"/>
    public string? GetDescription()
    {
        return GetPropertyValue("Description");
    }

    /// <summary>
    /// Get the name of the package.
    /// </summary>
    /// <remarks>
    /// Gets the value of the following properties in order of precedence: `PackageName`, `AssemblyName`, file name.
    /// </remarks>
    /// <returns>The name of the package, or <see langword="null"/> if not set.</returns>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target"/>
    public string GetPackageName()
    {
        string? name = GetPropertyValue("PackageName");
        if (name is not null)
            return name;
        name = GetPropertyValue("AssemblyName");
        if (name is not null)
            return name;
        name = Path.GetFileName(FilePath);
        if(name.EndsWith(".csproj"))
            name = name.Substring(0, name.Length - 7);
        return name;
    }

    private string? GetPropertyValue(string propertyName)
    {
        if (_xDocument.Root is null)
            return null;
        IEnumerable<XElement> propertyGroups = _xDocument
            .Root
            .Elements("PropertyGroup");
        IEnumerable<string> properties = propertyGroups
            .Elements(propertyName)
            .Select(x => x.Value);
        return properties.FirstOrDefault();
    }
}
