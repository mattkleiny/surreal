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
    return ValueTask.FromResult<Stream>(File.Open(path, FileMode.Open));
  }

  public override ValueTask<Stream> OpenOutputStreamAsync(string path)
  {
    return ValueTask.FromResult<Stream>(File.Open(path, FileMode.OpenOrCreate));
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
    return new PathWatcher(path);
  }

  private sealed class PathWatcher : IPathWatcher
  {
    private readonly FileSystemWatcher watcher;

    public VirtualPath Path { get; }

    public event Action<VirtualPath>? Created;
    public event Action<VirtualPath>? Modified;
    public event Action<VirtualPath>? Deleted;

    public PathWatcher(VirtualPath path)
    {
      watcher = new FileSystemWatcher(path.Target.ToString()!);

      // adapt the event interface
      watcher.Created += (_, _) => Created?.Invoke(path);
      watcher.Changed += (_, _) => Modified?.Invoke(path);
      watcher.Renamed += (_, _) => Modified?.Invoke(path);
      watcher.Deleted += (_, _) => Deleted?.Invoke(path);

      Path = path;
    }

    public void Dispose()
    {
      watcher.Dispose();
    }
  }
}
