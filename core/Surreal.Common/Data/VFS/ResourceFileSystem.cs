using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Surreal.Data.VFS {
  public sealed class ResourceFileSystem : FileSystem {
    private static Assembly[] GetDefaultAssemblies() => AppDomain.CurrentDomain
        .GetAssemblies()
        // dynamic assemblies do not support resources
        .Where(assembly => !assembly.IsDynamic)
        .ToArray();

    private readonly Assembly[] assemblies;

    public ResourceFileSystem()
        : this(GetDefaultAssemblies()) {
    }

    public ResourceFileSystem(params Assembly[] assemblies)
        : base("resource", "resources", "embedded", "resx") {
      this.assemblies = assemblies;
    }

    public override Path Resolve(string root, params string[] paths) => string.Join(root, ".", string.Join(".", paths));

    public override Task<Path[]> EnumerateAsync(string path, string wildcard) => throw new NotSupportedException();

    public override Task<Size> GetSizeAsync(string path)     => throw new NotSupportedException();
    public override Task<bool> IsFileAsync(string path)      => throw new NotSupportedException();
    public override Task<bool> IsDirectoryAsync(string path) => throw new NotSupportedException();
    public override Task<bool> ExistsAsync(string path)      => throw new NotSupportedException();

    public override Task<Stream> OpenInputStreamAsync(string path) {
      foreach (var assembly in assemblies) {
        var stream = assembly.GetManifestResourceStream(NormalizePath(path));

        if (stream != null) {
          return Task.FromResult(stream);
        }
      }

      throw new ResourceNotFoundException("Unable to locate resource: " + path);
    }

    public override Task<Stream> OpenOutputStreamAsync(string path) => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string NormalizePath(string path) => path.Replace('/', '.');
  }
}