using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Surreal.Memory;

namespace Surreal.IO
{
  public sealed class ResourceFileSystem : FileSystem
  {
    private static Assembly[] GetDefaultAssemblies() => AppDomain.CurrentDomain
        .GetAssemblies()
        // dynamic assemblies do not support resources
        .Where(assembly => !assembly.IsDynamic)
        .ToArray();

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

    public override Path Resolve(string root, params string[] paths) => string.Join(root, ".", string.Join(".", paths));

    public override ValueTask<Path[]> EnumerateAsync(string path, string wildcard) => throw new NotSupportedException();

    public override ValueTask<Size> GetSizeAsync(string path)     => throw new NotSupportedException();
    public override ValueTask<bool> IsFileAsync(string path)      => throw new NotSupportedException();
    public override ValueTask<bool> IsDirectoryAsync(string path) => throw new NotSupportedException();
    public override ValueTask<bool> ExistsAsync(string path)      => throw new NotSupportedException();

    public override ValueTask<Stream> OpenInputStreamAsync(string path)
    {
      foreach (var assembly in assemblies)
      {
        var stream = assembly.GetManifestResourceStream(NormalizePath(path));

        if (stream != null)
        {
          return ValueTask.FromResult(stream);
        }
      }

      throw new ResourceNotFoundException("Unable to locate resource: " + path);
    }

    public override ValueTask<Stream> OpenOutputStreamAsync(string path) => throw new NotSupportedException();

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
}