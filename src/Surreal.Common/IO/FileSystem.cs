using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Surreal.Collections;
using Surreal.Memory;

namespace Surreal.IO
{
  /// <summary>Allows watching <see cref="Path"/>s for changes.</summary>
  public interface IPathWatcher : IDisposable
  {
    Path Path { get; }

    event Action<Path> Created;
    event Action<Path> Modified;
    event Action<Path> Deleted;
  }

  /// <summary>Represents some virtual file system.</summary>
  public interface IFileSystem
  {
    ISet<string> Schemes         { get; }
    bool         SupportsWatcher { get; }

    Path Resolve(string root, params string[] paths);

    ValueTask<Path[]> EnumerateAsync(string path, string wildcard);

    ValueTask<Size> GetSizeAsync(string path);
    ValueTask<bool> ExistsAsync(string path);
    ValueTask<bool> IsFileAsync(string path);
    ValueTask<bool> IsDirectoryAsync(string path);

    ValueTask<Stream> OpenInputStreamAsync(string path);
    ValueTask<Stream> OpenOutputStreamAsync(string path);

    IPathWatcher WatchPath(Path path);
  }

  /// <summary>A registry for <see cref="IFileSystem"/>s.</summary>
  public interface IFileSystemRegistry
  {
    ReadOnlySlice<IFileSystem> GetByScheme(string scheme);

    void Add(IFileSystem system);
    void Clear();
  }

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

    public abstract Path Resolve(string root, params string[] paths);

    public abstract ValueTask<Path[]> EnumerateAsync(string path, string wildcard);

    public abstract ValueTask<Size> GetSizeAsync(string path);
    public abstract ValueTask<bool> IsDirectoryAsync(string path);
    public abstract ValueTask<bool> ExistsAsync(string path);
    public abstract ValueTask<bool> IsFileAsync(string path);

    public abstract ValueTask<Stream> OpenInputStreamAsync(string path);
    public abstract ValueTask<Stream> OpenOutputStreamAsync(string path);

    public virtual IPathWatcher WatchPath(Path path)
    {
      throw new NotSupportedException("This file system does not support path watching.");
    }

    private sealed class FileSystemRegistry : IFileSystemRegistry
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
}
