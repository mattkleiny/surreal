using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Surreal.IO.VFS {
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
}