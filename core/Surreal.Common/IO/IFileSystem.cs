using Surreal.Memory;

namespace Surreal.IO;

/// <summary>Represents some virtual file system.</summary>
public interface IFileSystem
{
  ISet<string> Schemes         { get; }
  bool         SupportsWatcher { get; }

  VirtualPath Resolve(string root, params string[] paths);

  ValueTask<VirtualPath[]> EnumerateAsync(string path, string wildcard);

  ValueTask<Size> GetSizeAsync(string path);
  ValueTask<bool> ExistsAsync(string path);
  ValueTask<bool> IsFileAsync(string path);
  ValueTask<bool> IsDirectoryAsync(string path);

  ValueTask<Stream> OpenInputStreamAsync(string path);
  ValueTask<Stream> OpenOutputStreamAsync(string path);

  IPathWatcher WatchPath(VirtualPath path);
}

/// <summary>A registry for <see cref="IFileSystem"/>s.</summary>
public interface IFileSystemRegistry
{
  IFileSystem? GetByScheme(string scheme);

  void Add(IFileSystem system);
  void Remove(IFileSystem system);

  void Clear();
}
