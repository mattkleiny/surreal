using System;
using Surreal.IO;

namespace Surreal.Compute.Memory {
  public abstract class ComputeBuffer : ComputeResource, IHardwareBuffer {
    public int Length { get; protected set; }

    public abstract Span<T> Read<T>(Range range)
        where T : unmanaged;

    public abstract void Write<T>(Span<T> data)
        where T : unmanaged;
  }
}