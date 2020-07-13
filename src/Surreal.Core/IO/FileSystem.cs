using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Surreal.Collections;

namespace Surreal.IO {
  public interface IFileSystem {
    string       Name    { get; }
    ISet<string> Schemes { get; }

    bool         SupportsWatcher { get; }
    IPathWatcher WatchPath(Path path);

    Path Resolve(string root, params string[] paths);

    Task<Path[]> EnumerateAsync(string path, string wildcard);

    Task<Size> GetSizeAsync(string path);
    Task<bool> ExistsAsync(string path);
    Task<bool> IsFileAsync(string path);
    Task<bool> IsDirectoryAsync(string path);

    Task<Stream> OpenInputStreamAsync(string path);
    Task<Stream> OpenOutputStreamAsync(string path);
  }

  public abstract class FileSystem : IFileSystem {
    protected FileSystem(params string[] schemes) {
      Check.That(schemes.Length > 0, "schemes.Length > 0");

      Name    = schemes[0];
      Schemes = new HashSet<string>(schemes);
    }

    public string       Name    { get; }
    public ISet<string> Schemes { get; }

    public virtual bool SupportsWatcher => false;

    public abstract Task<Path[]> EnumerateAsync(string path, string wildcard);

    public abstract Task<Size> GetSizeAsync(string path);
    public abstract Task<bool> IsDirectoryAsync(string path);
    public abstract Task<bool> ExistsAsync(string path);
    public abstract Task<bool> IsFileAsync(string path);

    public abstract Path Resolve(string root, params string[] paths);

    public abstract Task<Stream> OpenInputStreamAsync(string path);
    public abstract Task<Stream> OpenOutputStreamAsync(string path);

    public virtual IPathWatcher WatchPath(Path path) {
      throw new NotSupportedException("This file system does not support path watching.");
    }
  }

  public static class FileSystems {
    public static readonly IFileSystemRegistry Registry = new FileSystemRegistry();

    public static IFileSystem GetForScheme(string scheme) => Registry.GetByScheme(scheme).FirstOrDefault();

    private sealed class FileSystemRegistry : IFileSystemRegistry {
      private readonly IMultiDictionary<string, IFileSystem> fileSystemByScheme = new MultiDictionary<string, IFileSystem>(StringComparer.OrdinalIgnoreCase);

      public void Add(IFileSystem system) {
        foreach (var scheme in system.Schemes) {
          fileSystemByScheme.Add(scheme, system);
        }
      }

      public void Clear() => fileSystemByScheme.Clear();

      public IEnumerable<IFileSystem> GetByScheme(string scheme) => fileSystemByScheme[scheme];

      public IEnumerator<IFileSystem> GetEnumerator() => fileSystemByScheme.Values.GetEnumerator();
      IEnumerator IEnumerable.        GetEnumerator() => GetEnumerator();
    }
  }
}