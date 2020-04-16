using System;

namespace Surreal.IO
{
  public interface IPathWatcher : IDisposable
  {
    event Action<Path> Created;
    event Action<Path> Modified;
    event Action<Path> Deleted;
  }
}
