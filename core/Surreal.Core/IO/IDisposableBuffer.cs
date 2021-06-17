using System;

namespace Surreal.IO {
  public interface IDisposableBuffer<T> : IBuffer<T>, IDisposable
      where T : unmanaged {
  }
}