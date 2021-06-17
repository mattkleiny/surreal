using System;

namespace Surreal.Data.VFS {
  public interface IPathWatcher : IDisposable {
    Path Path { get; }

    event Action<Path> Created;
    event Action<Path> Modified;
    event Action<Path> Deleted;
  }
}