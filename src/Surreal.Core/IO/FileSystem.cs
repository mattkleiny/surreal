using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Surreal.Memory;

namespace Surreal.IO
{
  public abstract class FileSystem : IFileSystem
  {
    protected FileSystem(params string[] schemes)
    {
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

    public virtual IPathWatcher WatchPath(Path path)
    {
      throw new NotSupportedException("This file system does not support path watching.");
    }
  }
}