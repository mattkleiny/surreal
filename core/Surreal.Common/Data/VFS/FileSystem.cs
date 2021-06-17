using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Surreal.Data.VFS {
  public interface IFileSystem {
    ISet<string> Schemes         { get; }
    bool         SupportsWatcher { get; }

    Path Resolve(string root, params string[] paths);

    Task<Path[]> EnumerateAsync(string path, string wildcard);

    Task<Size> GetSizeAsync(string path);
    Task<bool> ExistsAsync(string path);
    Task<bool> IsFileAsync(string path);
    Task<bool> IsDirectoryAsync(string path);

    Task<Stream> OpenInputStreamAsync(string path);
    Task<Stream> OpenOutputStreamAsync(string path);

    IPathWatcher WatchPath(Path path);
  }

  public interface IPathWatcher : IDisposable {
    Path Path { get; }

    event Action<Path> Created;
    event Action<Path> Modified;
    event Action<Path> Deleted;
  }

  public abstract class FileSystem : IFileSystem {
    protected FileSystem(params string[] schemes) {
      Debug.Assert(schemes.Length > 0, "schemes.Length > 0");

      Schemes = new HashSet<string>(schemes);
    }

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
}