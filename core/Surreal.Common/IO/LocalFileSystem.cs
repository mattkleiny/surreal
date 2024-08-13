using System.IO.MemoryMappedFiles;
using Surreal.Memory;
using Surreal.Text;

namespace Surreal.IO;

/// <summary>
/// A <see cref="FileSystem" /> for the host operating system.
/// </summary>
public sealed class LocalFileSystem() : FileSystem("local")
{
  private const string Separator = "/";

  public override bool SupportsWatcher => true;
  public override bool SupportsMemoryMapping => true;

  public override VirtualPath Resolve(VirtualPath path, params string[] paths)
  {
    return path with { Target = $"{path.Target}{Separator}{string.Join(Separator, paths)}" };
  }

  public override string ToAbsolutePath(VirtualPath path)
  {
    return Path.GetFullPath(path.Target.ToString());
  }

  public override VirtualPath[] Enumerate(string path, string wildcard)
  {
    return Directory
      .GetFiles(path, wildcard, SearchOption.AllDirectories)
      .Select(x => new VirtualPath("local", x))
      .ToArray();
  }

  public override Size GetSize(string path)
  {
    return new Size(new FileInfo(path).Length);
  }

  public override bool Exists(string path)
  {
    return Directory.Exists(path) || File.Exists(path);
  }

  public override bool IsFile(string path)
  {
    var fileNameBegin = path.LastIndexOf(Path.DirectorySeparatorChar);
    if (fileNameBegin == -1) fileNameBegin = 0; // we could just be a top level file (test.txt)

    var fileName = path.AsStringSpan(fileNameBegin + 1);

    return fileName.IndexOf('.') != -1; // we have an extension
  }

  public override bool IsDirectory(string path)
  {
    var directoryNameBegin = path.LastIndexOf(Path.DirectorySeparatorChar);
    if (directoryNameBegin == -1) directoryNameBegin = 0; // we could just be a top level directory (test)

    var directoryName = path.AsStringSpan(directoryNameBegin + 1);

    return directoryName.IndexOf('.') == -1; // we don't have an extension
  }

  public override Stream OpenInputStream(string path)
  {
    return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
  }

  public override Stream OpenOutputStream(string path)
  {
    return File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
  }

  public override MemoryMappedFile OpenMemoryMappedFile(string path)
  {
    return MemoryMappedFile.CreateFromFile(
      path,
      FileMode.OpenOrCreate,
      Guid.NewGuid().ToString(), // needs to be unique
      0,
      MemoryMappedFileAccess.ReadWrite
    );
  }

  public override IPathWatcher WatchPath(VirtualPath path, bool includeSubPaths)
  {
    return new PathWatcher(path, includeSubPaths);
  }

  /// <summary>
  /// The <see cref="IPathWatcher"/> implementation for <see cref="LocalFileSystem"/>.
  /// </summary>
  private sealed class PathWatcher : IPathWatcher
  {
    private readonly FileSystemWatcher _watcher;

    public PathWatcher(VirtualPath directoryPath, bool includeSubPaths)
    {
      _watcher = new FileSystemWatcher(directoryPath.Target.ToString())
      {
        EnableRaisingEvents = true,
        IncludeSubdirectories = includeSubPaths
      };

      // adapt the event interface
      var context = SynchronizationContext.Current;
      if (context != null)
      {
        _watcher.Created += (_, args) => context.Post(_ => Created?.Invoke(args.FullPath), null);
        _watcher.Changed += (_, args) => context.Post(_ => Changed?.Invoke(args.FullPath, (PathChangeTypes)args.ChangeType), null);
        _watcher.Renamed += (_, args) => context.Post(_ => Renamed?.Invoke(args.OldFullPath, args.FullPath), null);
        _watcher.Deleted += (_, args) => context.Post(_ => Deleted?.Invoke(args.FullPath), null);
      }
      else
      {
        _watcher.Created += (_, args) => Created?.Invoke(args.FullPath);
        _watcher.Changed += (_, args) => Changed?.Invoke(args.FullPath, (PathChangeTypes)args.ChangeType);
        _watcher.Renamed += (_, args) => Renamed?.Invoke(args.OldFullPath, args.FullPath);
        _watcher.Deleted += (_, args) => Deleted?.Invoke(args.FullPath);
      }
    }

    public event Action<VirtualPath>? Created;
    public event Action<VirtualPath, PathChangeTypes>? Changed;
    public event Action<VirtualPath, VirtualPath>? Renamed;
    public event Action<VirtualPath>? Deleted;

    public void Dispose()
    {
      _watcher.Dispose();
    }
  }
}
