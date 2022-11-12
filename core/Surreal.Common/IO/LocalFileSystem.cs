using System.IO.MemoryMappedFiles;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>A <see cref="FileSystem" /> for the host operating system.</summary>
public sealed class LocalFileSystem : FileSystem
{
  private const string Separator = "/";

  public LocalFileSystem()
    : base("local")
  {
  }

  public override bool SupportsWatcher => true;
  public override bool SupportsMemoryMapping => true;

  public override VirtualPath Resolve(VirtualPath path, params string[] paths)
  {
    return path with { Target = $"{path.Target}{Separator}{string.Join(Separator, paths)}" };
  }

  public override VirtualPath[] Enumerate(string path, string wildcard)
  {
    return Directory
      .GetFiles(path, wildcard, SearchOption.AllDirectories)
      .Select(_ => new VirtualPath("local", _))
      .ToArray();
  }

  public override Size GetSize(string path)
  {
    return new Size(new FileInfo(path).Length);
  }

  public override bool Exists(string path)
  {
    return IsDirectory(path) || IsFile(path);
  }

  public override bool IsFile(string path)
  {
    return File.Exists(path);
  }

  public override bool IsDirectory(string path)
  {
    return Directory.Exists(path);
  }

  public override Stream OpenInputStream(string path)
  {
    return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
  }

  public override Stream OpenOutputStream(string path)
  {
    return File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
  }

  public override MemoryMappedFile OpenMemoryMappedFile(string path, int offset, int length)
  {
    return MemoryMappedFile.CreateFromFile(
      path,
      FileMode.OpenOrCreate,
      Guid.NewGuid().ToString(), // needs to be unique
      length,
      MemoryMappedFileAccess.ReadWrite
    );
  }

  public override IPathWatcher WatchPath(VirtualPath path)
  {
    return new PathWatcher(path.GetDirectory(), path);
  }

  private sealed class PathWatcher : IPathWatcher
  {
    private readonly FileSystemWatcher _watcher;

    public PathWatcher(VirtualPath directoryPath, VirtualPath filePath)
    {
      _watcher = new FileSystemWatcher(directoryPath.Target.ToString())
      {
        Filter = Path.GetFileName(filePath.Target.ToString()),

        EnableRaisingEvents = true
      };

      // adapt the event interface
      var context = SynchronizationContext.Current;
      if (context != null)
      {
        _watcher.Created += (_, _) => context.Post(_ => Created?.Invoke(filePath), null);
        _watcher.Changed += (_, _) => context.Post(_ => Modified?.Invoke(filePath), null);
        _watcher.Renamed += (_, _) => context.Post(_ => Modified?.Invoke(filePath), null);
        _watcher.Deleted += (_, _) => context.Post(_ => Deleted?.Invoke(filePath), null);
      }
      else
      {
        _watcher.Created += (_, _) => Created?.Invoke(filePath);
        _watcher.Changed += (_, _) => Modified?.Invoke(filePath);
        _watcher.Renamed += (_, _) => Modified?.Invoke(filePath);
        _watcher.Deleted += (_, _) => Deleted?.Invoke(filePath);
      }

      FilePath = filePath;
    }

    public event Action<VirtualPath>? Created;
    public event Action<VirtualPath>? Modified;
    public event Action<VirtualPath>? Deleted;

    public VirtualPath FilePath { get; }

    public void Dispose()
    {
      _watcher.Dispose();
    }
  }
}



