﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Surreal.IO.VFS {
  public sealed class LocalFileSystem : FileSystem {
    private static readonly string PathSeparator = System.IO.Path.PathSeparator.ToString();

    public LocalFileSystem()
        : base("local") {
    }

    public override bool SupportsWatcher => true;

    public override Task<Path[]> EnumerateAsync(string path, string wildcard) {
      var files = Directory
          .GetFiles(path, wildcard, SearchOption.AllDirectories)
          .Select(_ => new Path("local", _))
          .ToArray();

      return Task.FromResult(files);
    }

    public override Task<Size> GetSizeAsync(string path) {
      return Task.FromResult(new Size(new FileInfo(path).Length));
    }

    public override async Task<bool> ExistsAsync(string path) {
      return await IsDirectoryAsync(path) || await IsFileAsync(path);
    }

    public override Task<bool> IsDirectoryAsync(string path) {
      return Task.FromResult(Directory.Exists(path));
    }

    public override Task<bool> IsFileAsync(string path) {
      return Task.FromResult(File.Exists(path));
    }

    public override Path Resolve(string root, params string[] paths) {
      return string.Join(root, PathSeparator, string.Join(PathSeparator, paths));
    }

    public override Task<Stream> OpenInputStreamAsync(string path) {
      return Task.FromResult<Stream>(File.Open(path, FileMode.Open));
    }

    public override Task<Stream> OpenOutputStreamAsync(string path) {
      return Task.FromResult<Stream>(File.Open(path, FileMode.OpenOrCreate));
    }

    public override IPathWatcher WatchPath(Path path) => new PathWatcher(path);

    private sealed class PathWatcher : IPathWatcher {
      private readonly FileSystemWatcher watcher;

      public PathWatcher(Path path) {
        watcher = new FileSystemWatcher(path.Target);

        // adapt the event interface
        watcher.Created += (sender, args) => Created?.Invoke(path);
        watcher.Changed += (sender, args) => Modified?.Invoke(path);
        watcher.Renamed += (sender, args) => Modified?.Invoke(path);
        watcher.Deleted += (sender, args) => Deleted?.Invoke(path);

        Path = path;
      }

      public Path Path { get; }

      public event Action<Path> Created  = null!;
      public event Action<Path> Modified = null!;
      public event Action<Path> Deleted  = null!;

      public void Dispose() {
        watcher.Dispose();
      }
    }
  }
}