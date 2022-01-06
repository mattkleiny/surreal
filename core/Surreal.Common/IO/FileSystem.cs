using Surreal.Memory;

namespace Surreal.IO;

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

  private sealed class FileSystemRegistry : IFileSystemRegistry
  {
    private readonly Dictionary<string, IFileSystem> fileSystemByScheme = new(StringComparer.OrdinalIgnoreCase);

    public FileSystemRegistry()
    {
      Add(new LocalFileSystem());
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

    public void Clear()
    {
      fileSystemByScheme.Clear();
    }
  }
}
