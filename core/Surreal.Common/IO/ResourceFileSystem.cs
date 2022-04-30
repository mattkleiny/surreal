using System.Reflection;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>A <see cref="FileSystem"/> that uses embed assembly resources.</summary>
public sealed class ResourceFileSystem : FileSystem
{
  private const string Separator = "/";

  private static Assembly[] GetDefaultAssemblies()
  {
    return AppDomain.CurrentDomain
      .GetAssemblies()
      // dynamic assemblies do not support resources
      .Where(assembly => !assembly.IsDynamic)
      .ToArray();
  }

  private readonly Assembly[] assemblies;

  public ResourceFileSystem()
    : this(GetDefaultAssemblies())
  {
  }

  public ResourceFileSystem(params Assembly[] assemblies)
    : base("resource", "resources", "embedded", "resx")
  {
    this.assemblies = assemblies;
  }

  public override VirtualPath Resolve(VirtualPath path, params string[] paths)
    => path with { Target = $"{path.Target}{Separator}{string.Join(Separator, paths)}" };

  public override VirtualPath[] Enumerate(string path, string wildcard)
  {
    throw new NotSupportedException();
  }

  public override Size GetSize(string path)
  {
    throw new NotSupportedException();
  }

  public override bool Exists(string path)
  {
    throw new NotSupportedException();
  }

  public override bool IsFile(string path)
  {
    throw new NotSupportedException();
  }

  public override bool IsDirectory(string path)
  {
    throw new NotSupportedException();
  }

  public override Stream OpenInputStream(string path)
  {
    foreach (var assembly in assemblies)
    {
      var stream = assembly.GetManifestResourceStream(NormalizePath(path));
      if (stream != null)
      {
        return stream;
      }
    }

    throw new ResourceNotFoundException("Unable to locate resource: " + path);
  }

  public override Stream OpenOutputStream(string path)
  {
    throw new NotSupportedException();
  }

  private static string NormalizePath(string path)
  {
    return path.Replace('/', '.');
  }
}

public sealed class ResourceNotFoundException : Exception
{
  public ResourceNotFoundException(string message)
    : base(message)
  {
  }
}
