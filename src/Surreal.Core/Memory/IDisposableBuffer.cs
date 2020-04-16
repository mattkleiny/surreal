using System;

namespace Surreal.Memory
{
  public interface IDisposableBuffer<T> : IBuffer<T>, IDisposable
    where T : unmanaged
  {
  }
}
