using System.Buffers;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;

namespace Surreal.Platform.Internal;

internal sealed class OpenTKMemoryManager<T> : MemoryManager<T>
  where T : unmanaged
{
  private readonly int          id;
  private readonly BufferTarget target;
  private readonly int          size;
  private          IntPtr       address;

  public OpenTKMemoryManager(int id, BufferTarget target)
  {
    this.id     = id;
    this.target = target;

    GL.BindBuffer(target, id);
    GL.GetBufferParameter(target, BufferParameterName.BufferSize, out int size);

    address = GL.MapBuffer(target, BufferAccess.ReadWrite);

    if (address == IntPtr.Zero)
    {
      throw new Exception("Failed to map OpenGL buffer address space!");
    }

    this.size = size / Unsafe.SizeOf<T>();
  }

  public override unsafe Span<T> GetSpan()
  {
    if (address == IntPtr.Zero)
    {
      throw new Exception("Attempted to access unmapped OpenGL buffer!");
    }

    return new Span<T>(address.ToPointer(), size);
  }

  public override unsafe MemoryHandle Pin(int elementIndex = 0)
  {
    var address = (T*)this.address.ToPointer() + elementIndex;

    return new MemoryHandle(address);
  }

  public override void Unpin()
  {
    // no-op
  }

  protected override void Dispose(bool disposing)
  {
    GL.BindBuffer(target, id);
    GL.UnmapBuffer(target);

    address = IntPtr.Zero;
  }
}