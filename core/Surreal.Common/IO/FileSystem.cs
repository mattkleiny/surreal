using System.IO.MemoryMappedFiles;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>
/// Represents some virtual file system.
/// </summary>
public interface IFileSystem
{
  ISet<string> Schemes { get; }

  bool SupportsWatcher { get; }
  bool SupportsMemoryMapping { get; }

  VirtualPath Resolve(VirtualPath path, params string[] paths);

  // synchronous API
  VirtualPath[] Enumerate(string path, string wildcard);
  Size GetSize(string path);
  bool Exists(string path);
  bool IsFile(string path);
  bool IsDirectory(string path);
  Stream OpenInputStream(string path);
  Stream OpenOutputStream(string path);
  MemoryMappedFile OpenMemoryMappedFile(string path);
  IPathWatcher WatchPath(VirtualPath path, bool includeSubPaths);

  // asynchronous API
}

/// <summary>
/// A registry for <see cref="IFileSystem" />s.
/// </summary>
public interface IFileSystemRegistry
{
  IFileSystem? GetByScheme(string scheme);

  void Add(IFileSystem system);
  void Remove(IFileSystem system);

  void Clear();
}

/// <summary>
/// Allows watching <see cref="Path" />s for changes.
/// </summary>
public interface IPathWatcher : IDisposable
{
  event Action<VirtualPath> Created;
  event Action<VirtualPath, PathChangeTypes> Changed;
  event Action<VirtualPath, VirtualPath>? Renamed;
  event Action<VirtualPath> Deleted;
}

/// <summary>
/// A type of change for <see cref="IPathWatcher.Changed"/>.
/// </summary>
[Flags]
public enum PathChangeTypes
{
  None = 0,
  Created = 1 << 0,
  Deleted = 1 << 1,
  Modified = 1 << 2,
  Renamed = 1 << 3,
  All = Renamed | Modified | Deleted | Created
}

/// <summary>
/// Base class for any <see cref="IFileSystem" />.
/// </summary>
public abstract class FileSystem : IFileSystem
{
  protected FileSystem(params string[] schemes)
  {
    Debug.Assert(schemes.Length > 0, "schemes.Length > 0");

    Schemes = new HashSet<string>(schemes);
  }

  public static IFileSystemRegistry Registry { get; } = new FileSystemRegistry();

  public ISet<string> Schemes { get; }

  public virtual bool SupportsWatcher => false;
  public virtual bool SupportsMemoryMapping => false;

  public abstract VirtualPath Resolve(VirtualPath path, params string[] paths);

  // synchronous API
  public abstract VirtualPath[] Enumerate(string path, string wildcard);
  public abstract Size GetSize(string path);
  public abstract bool Exists(string path);
  public abstract bool IsFile(string path);
  public abstract bool IsDirectory(string path);
  public abstract Stream OpenInputStream(string path);
  public abstract Stream OpenOutputStream(string path);

  public virtual MemoryMappedFile OpenMemoryMappedFile(string path)
  {
    throw new NotSupportedException("This file system does not support memory mapping.");
  }

  public virtual IPathWatcher WatchPath(VirtualPath path, bool includeSubPaths)
  {
    throw new NotSupportedException("This file system does not support path watching.");
  }

  // asynchronous API

  public static IFileSystem? GetForScheme(string scheme)
  {
    return Registry.GetByScheme(scheme);
  }

  private sealed class FileSystemRegistry : IFileSystemRegistry
  {
    private readonly Dictionary<string, IFileSystem> _fileSystemByScheme = new(StringComparer.OrdinalIgnoreCase);

    public FileSystemRegistry()
    {
      Add(new LocalFileSystem());
      Add(new ResourceFileSystem());
    }

    public IFileSystem? GetByScheme(string scheme)
    {
      if (!_fileSystemByScheme.TryGetValue(scheme, out var fileSystem))
      {
        return default;
      }

      return fileSystem;
    }

    public void Add(IFileSystem system)
    {
      foreach (var scheme in system.Schemes)
      {
        _fileSystemByScheme[scheme] = system;
      }
    }

    public void Remove(IFileSystem system)
    {
      foreach (var scheme in system.Schemes)
      {
        _fileSystemByScheme.Remove(scheme);
      }
    }

    public void Clear()
    {
      _fileSystemByScheme.Clear();
    }
  }
}
