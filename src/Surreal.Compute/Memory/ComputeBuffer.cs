using System;
using System.Buffers;

namespace Surreal.Compute.Memory {
  public abstract class ComputeBuffer : ComputeResource {
    public int Length { get; protected set; }

    public abstract Memory<T> Read<T>(Range range)
        where T : unmanaged;

    public abstract void Write<T>(Span<T> data)
        where T : unmanaged;

    public abstract MemoryManager<T> Pin<T>()
        where T : unmanaged;
  }
}