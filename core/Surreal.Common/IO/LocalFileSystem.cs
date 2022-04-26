using System.IO.MemoryMappedFiles;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>A <see cref="FileSystem"/> for the host operating system.</summary>
public sealed class LocalFileSystem : FileSystem
{
  private const string Separator = "/";

  public LocalFileSystem()
    : base("local")
  {
  }

  public override bool SupportsWatcher       => true;
  public override bool SupportsMemoryMapping => true;

  public override VirtualPath Resolve(VirtualPath path, params string[] paths)
    => path with { Target = $"{path.Target}{Separator}{string.Join(Separator, paths)}" };

  public override ValueTask<VirtualPath[]> EnumerateAsync(string path, string wildcard)
  {
    var files = Directory
      .GetFiles(path, wildcard, SearchOption.AllDirectories)
      .Select(_ => new VirtualPath("local", _))
      .ToArray();

    return ValueTask.FromResult(files);
  }

  public override ValueTask<Size> GetSizeAsync(string path)
  {
    return ValueTask.FromResult(new Size(new FileInfo(path).Length));
  }

  public override async ValueTask<bool> ExistsAsync(string path)
  {
    return await IsDirectoryAsync(path) || await IsFileAsync(path);
  }

  public override ValueTask<bool> IsDirectoryAsync(string path)
  {
    return ValueTask.FromResult(Directory.Exists(path));
  }

  public override ValueTask<bool> IsFileAsync(string path)
  {
    return ValueTask.FromResult(File.Exists(path));
  }

  public override ValueTask<Stream> OpenInputStreamAsync(string path)
  {
    return ValueTask.FromResult<Stream>(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));
  }

  public override ValueTask<Stream> OpenOutputStreamAsync(string path)
  {
    return ValueTask.FromResult<Stream>(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
  }

  public override MemoryMappedFile OpenMemoryMappedFile(string path, int offset, int length)
  {
    return MemoryMappedFile.CreateFromFile(
      path: path,
      mode: FileMode.OpenOrCreate,
      mapName: Guid.NewGuid().ToString(), // needs to be unique
      capacity: length,
      access: MemoryMappedFileAccess.ReadWrite
    );
  }

  public override IPathWatcher WatchPath(VirtualPath path)
  {
    return new PathWatcher(path.GetDirectory(), path);
  }

  private sealed class PathWatcher : IPathWatcher
  {
    private readonly FileSystemWatcher watcher;

    public event Action<VirtualPath>? Created;
    public event Action<VirtualPath>? Modified;
    public event Action<VirtualPath>? Deleted;

    public VirtualPath FilePath { get; }

    public PathWatcher(VirtualPath directoryPath, VirtualPath filePath)
    {
      watcher = new FileSystemWatcher(directoryPath.Target.ToString())
      {
        Filter = Path.GetFileName(filePath.Target.ToString()),

        EnableRaisingEvents = true,
      };

      // adapt the event interface
      var context = SynchronizationContext.Current;
      if (context != null)
      {
        watcher.Created += (_, _) => context.Post(_ => Created?.Invoke(filePath), null);
        watcher.Changed += (_, _) => context.Post(_ => Modified?.Invoke(filePath), null);
        watcher.Renamed += (_, _) => context.Post(_ => Modified?.Invoke(filePath), null);
        watcher.Deleted += (_, _) => context.Post(_ => Deleted?.Invoke(filePath), null);
      }
      else
      {
        watcher.Created += (_, _) => Created?.Invoke(filePath);
        watcher.Changed += (_, _) => Modified?.Invoke(filePath);
        watcher.Renamed += (_, _) => Modified?.Invoke(filePath);
        watcher.Deleted += (_, _) => Deleted?.Invoke(filePath);
      }

      FilePath = filePath;
    }

    public void Dispose()
    {
      watcher.Dispose();
    }
  }
}
