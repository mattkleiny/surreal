using System;

namespace Surreal.IO {
  public interface IBuffer<T>
      where T : unmanaged {
    int     Length { get; }
    int     Stride { get; }
    Size    Size   { get; }
    Span<T> Span   { get; }

    void Clear();
    void Fill(T value);

    IBuffer<T> Slice(int offset, int size);
  }
}