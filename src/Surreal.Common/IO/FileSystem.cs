using System.Diagnostics;
using Surreal.Collections;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>Base class for any <see cref="IFileSystem"/>.</summary>
public abstract class FileSystem : IFileSystem
{
  public static IFileSystemRegistry Registry { get; } = new FileSystemRegistry();

  public static IFileSystem? GetForScheme(string scheme)
  {
    var systems = Registry.GetByScheme(scheme);

    if (systems.Length > 0)
    {
      return systems[0];
    }

    return default;
  }

  protected FileSystem(params string[] schemes)
  {
    Debug.Assert(schemes.Length > 0, "schemes.Length > 0");

    Schemes = new HashSet<string>(schemes);
  }

  public ISet<string> Schemes { get; }

  public virtual bool SupportsWatcher => false;

  public abstract VirtualPath Resolve(string root, params string[] paths);

  public abstract ValueTask<VirtualPath[]> EnumerateAsync(string path, string wildcard);

  public abstract ValueTask<Size> GetSizeAsync(string path);
  public abstract ValueTask<bool> IsDirectoryAsync(string path);
  public abstract ValueTask<bool> ExistsAsync(string path);
  public abstract ValueTask<bool> IsFileAsync(string path);

  public abstract ValueTask<Stream> OpenInputStreamAsync(string path);
  public abstract ValueTask<Stream> OpenOutputStreamAsync(string path);

  public virtual IPathWatcher WatchPath(VirtualPath path)
  {
    throw new NotSupportedException("This file system does not support path watching.");
  }

  public sealed class FileSystemRegistry : IFileSystemRegistry
  {
    private readonly MultiDictionary<string, IFileSystem> fileSystemByScheme = new(StringComparer.OrdinalIgnoreCase);

    public ReadOnlySlice<IFileSystem> GetByScheme(string scheme)
    {
      return fileSystemByScheme[scheme];
    }

    public void Add(IFileSystem system)
    {
      foreach (var scheme in system.Schemes)
      {
        fileSystemByScheme.Add(scheme, system);
      }
    }

    public void Clear()
    {
      fileSystemByScheme.Clear();
    }
  }
}
