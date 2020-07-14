using System;

namespace Surreal.IO {
  public interface IHardwareBuffer {
    Span<T> Read<T>(Range range)
        where T : unmanaged;

    void Write<T>(Span<T> data)
        where T : unmanaged;
  }
}