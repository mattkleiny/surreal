using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Surreal.Compute;

namespace Surreal.Internal.Compute;

internal sealed class OpenTKComputeServer : IComputeServer
{
  public ComputeHandle CreateBuffer()
  {
    var buffer = GL.GenBuffer();

    return new ComputeHandle(buffer.Handle);
  }

  public void DeleteBuffer(ComputeHandle handle)
  {
    var buffer = new BufferHandle(handle);

    GL.DeleteBuffer(buffer);
  }

  public unsafe Memory<T> ReadBufferData<T>(ComputeHandle handle, Range range) where T : unmanaged
  {
    var buffer = new BufferHandle(handle);

    int sizeInBytes = 0;

    GL.BindBuffer(BufferTargetARB.ArrayBuffer, buffer);
    GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref sizeInBytes);

    var stride = sizeof(T);
    var (offset, length) = range.GetOffsetAndLength(sizeInBytes / stride);

    var byteOffset = offset * stride;
    var result     = new T[length];

    fixed (T* pointer = result)
    {
      GL.GetBufferSubData(BufferTargetARB.ArrayBuffer, new IntPtr(byteOffset), sizeInBytes, pointer);
    }

    return result;
  }

  public unsafe void WriteBufferData<T>(ComputeHandle handle, ReadOnlySpan<T> data) where T : unmanaged
  {
    var buffer = new BufferHandle(handle);
    var bytes  = data.Length * sizeof(T);

    fixed (T* pointer = data)
    {
      GL.BindBuffer(BufferTargetARB.ArrayBuffer, buffer);
      GL.BufferData(BufferTargetARB.ArrayBuffer, bytes, pointer, BufferUsageARB.DynamicCopy);
    }
  }
}
