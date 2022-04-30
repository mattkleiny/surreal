using System.IO.MemoryMappedFiles;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>Represents some virtual file system.</summary>
public interface IFileSystem
{
  ISet<string> Schemes { get; }

  bool SupportsWatcher       { get; }
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
  MemoryMappedFile OpenMemoryMappedFile(string path, int offset, int length);
  IPathWatcher WatchPath(VirtualPath path);

  // asynchronous API
  ValueTask<VirtualPath[]> EnumerateAsync(string path, string wildcard);
  ValueTask<Size> GetSizeAsync(string path);
  ValueTask<bool> ExistsAsync(string path);
  ValueTask<bool> IsFileAsync(string path);
  ValueTask<bool> IsDirectoryAsync(string path);
  ValueTask<Stream> OpenInputStreamAsync(string path);
  ValueTask<Stream> OpenOutputStreamAsync(string path);
}

/// <summary>A registry for <see cref="IFileSystem"/>s.</summary>
public interface IFileSystemRegistry
{
  IFileSystem? GetByScheme(string scheme);

  void Add(IFileSystem system);
  void Remove(IFileSystem system);

  void Clear();
}

/// <summary>Allows watching <see cref="Path"/>s for changes.</summary>
public interface IPathWatcher : IDisposable
{
  /// <summary>The <see cref="VirtualPath"/> being watched.</summary>
  VirtualPath FilePath { get; }

  event Action<VirtualPath> Created;
  event Action<VirtualPath> Modified;
  event Action<VirtualPath> Deleted;
}

/// <summary>Base class for any <see cref="IFileSystem"/>.</summary>
public abstract class FileSystem : IFileSystem
{
  public static IFileSystemRegistry Registry { get; } = new FileSystemRegistry();

  public static IFileSystem? GetForScheme(string scheme)
  {
    return Registry.GetByScheme(scheme);
  }

  protected FileSystem(params string[] schemes)
  {
    Debug.Assert(schemes.Length > 0, "schemes.Length > 0");

    Schemes = new HashSet<string>(schemes);
  }

  public ISet<string> Schemes { get; }

  public virtual bool SupportsWatcher       => false;
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

  public virtual MemoryMappedFile OpenMemoryMappedFile(string path, int offset, int length)
    => throw new NotSupportedException("This file system does not support memory mapping.");

  public virtual IPathWatcher WatchPath(VirtualPath path)
    => throw new NotSupportedException("This file system does not support path watching.");

  // asynchronous API
  public virtual ValueTask<VirtualPath[]> EnumerateAsync(string path, string wildcard) => new(Enumerate(path, wildcard));
  public virtual ValueTask<Size> GetSizeAsync(string path) => new(GetSize(path));
  public virtual ValueTask<bool> IsDirectoryAsync(string path) => new(IsDirectory(path));
  public virtual ValueTask<bool> ExistsAsync(string path) => new(Exists(path));
  public virtual ValueTask<bool> IsFileAsync(string path) => new(IsFile(path));
  public virtual ValueTask<Stream> OpenInputStreamAsync(string path) => new(OpenInputStream(path));
  public virtual ValueTask<Stream> OpenOutputStreamAsync(string path) => new(OpenOutputStream(path));

  private sealed class FileSystemRegistry : IFileSystemRegistry
  {
    private readonly Dictionary<string, IFileSystem> fileSystemByScheme = new(StringComparer.OrdinalIgnoreCase);

    public FileSystemRegistry()
    {
      Add(new LocalFileSystem());
      Add(new ResourceFileSystem());
    }

    public IFileSystem? GetByScheme(string scheme)
    {
      if (!fileSystemByScheme.TryGetValue(scheme, out var fileSystem))
      {
        return default;
      }

      return fileSystem;
    }

    public void Add(IFileSystem system)
    {
      foreach (var scheme in system.Schemes)
      {
        fileSystemByScheme[scheme] = system;
      }
    }

    public void Remove(IFileSystem system)
    {
      foreach (var scheme in system.Schemes)
      {
        fileSystemByScheme.Remove(scheme);
      }
    }

    public void Clear()
    {
      fileSystemByScheme.Clear();
    }
  }
}
