using System.Buffers;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Surreal.IO;

namespace Surreal.Memory;

/// <summary>Represents a buffer of data of <see cref="T"/>.</summary>
public interface IBuffer<T>
{
  /// <summary>The underlying <see cref="Memory{T}"/> representing the buffer data.</summary>
  Memory<T> Memory { get; }

  /// <summary>The underlying <see cref="Span{T}"/> representing the buffer data.</summary>
  Span<T> Span { get; }

  /// <summary>Resizes the underlying buffer storage.</summary>
  void Resize(int newLength);
}

/// <summary>A <see cref="IBuffer{T}"/> that can be deterministically disposed.</summary>
public interface IDisposableBuffer<T> : IBuffer<T>, IDisposable
{
}

/// <summary>Static factories for <see cref="IBuffer{T}"/>s.</summary>
public static class Buffers
{
  public static IBuffer<T> Allocate<T>(int length)
    => new ManagedBuffer<T>(length);

  public static IBuffer<T> AllocatePinned<T>(int length, bool zeroFill = false)
    => new PinnedBuffer<T>(length, zeroFill);

  public static IDisposableBuffer<T> AllocateNative<T>(int length, bool zeroFill = false)
    where T : unmanaged => new NativeBuffer<T>(length, zeroFill);

  public static IDisposableBuffer<T> AllocateMapped<T>(VirtualPath path, int offset, int length)
    where T : unmanaged => new MappedBuffer<T>(path, offset, length);

  /// <summary>A buffer backed by a managed array.</summary>
  private sealed class ManagedBuffer<T> : IBuffer<T>
  {
    private T[] elements;

    public ManagedBuffer(int length)
    {
      elements = new T[length];
    }

    public Memory<T> Memory => elements;
    public Span<T>   Span   => elements;

    public void Resize(int newLength)
    {
      Array.Resize(ref elements, newLength);
    }
  }

  /// <summary>A buffer backed by a pinned array.</summary>
  private sealed class PinnedBuffer<T> : IBuffer<T>
  {
    private readonly T[] elements;

    public PinnedBuffer(int length, bool zeroFill)
    {
      elements = zeroFill
        ? GC.AllocateArray<T>(length, pinned: true)
        : GC.AllocateUninitializedArray<T>(length, pinned: true);
    }

    public Memory<T> Memory => elements;
    public Span<T>   Span   => elements;

    public void Resize(int newLength)
    {
      throw new NotSupportedException("Unable to resize pinned buffer!");
    }
  }

  /// <summary>A buffer backed by native memory.</summary>
  [SuppressMessage("Reliability", "CA2015:Do not define finalizers for types derived from MemoryManager<T>")]
  private sealed unsafe class NativeBuffer<T> : MemoryManager<T>, IDisposableBuffer<T>
    where T : unmanaged
  {
    private readonly int length;
    private void* buffer;

    private bool isDisposed;

    public NativeBuffer(int length, bool zeroFill)
    {
      this.length = length;

      buffer = zeroFill
        ? NativeMemory.AllocZeroed((nuint) length, (nuint) Unsafe.SizeOf<T>())
        : NativeMemory.Alloc((nuint) length, (nuint) Unsafe.SizeOf<T>());
    }

    ~NativeBuffer()
    {
      Dispose(false);
    }

    public Span<T> Span => GetSpan();

    public override Span<T> GetSpan()
    {
      CheckNotDisposed();

      return new Span<T>(buffer, length);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
      return default; // no-op
    }

    public override void Unpin()
    {
      // no-op
    }

    public void Resize(int newLength)
    {
      buffer = NativeMemory.Realloc(buffer, (nuint) newLength);
    }

    protected override void Dispose(bool disposing)
    {
      if (!isDisposed)
      {
        NativeMemory.Free(buffer);

        isDisposed = true;
      }
    }

    [Conditional("DEBUG")]
    private void CheckNotDisposed()
    {
      if (isDisposed)
      {
        throw new ObjectDisposedException(nameof(NativeBuffer<T>));
      }
    }

    Memory<T> IBuffer<T>.Memory => base.Memory;
  }

  /// <summary>A <see cref="IDisposableBuffer{T}"/> for memory mapped files.</summary>
  private sealed unsafe class MappedBuffer<T> : MemoryManager<T>, IDisposableBuffer<T>
    where T : unmanaged
  {
    private readonly MemoryMappedFile file;
    private readonly MemoryMappedViewAccessor accessor;
    private readonly byte* pointer;

    private bool isDisposed;

    public MappedBuffer(VirtualPath path, int offset, int length)
    {
      if (!path.SupportsMemoryMapping())
      {
        throw new InvalidOperationException($"The given path does not support memory mapped files: {path}");
      }

      file     = path.OpenMemoryMappedFile(offset, length);
      accessor = file.CreateViewAccessor(offset, length, MemoryMappedFileAccess.ReadWrite);

      accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
    }

    public Span<T> Span => GetSpan();

    public override Span<T> GetSpan()
    {
      CheckNotDisposed();

      return new Span<T>(pointer, (int) accessor.Capacity);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
      return default; // no-op
    }

    public override void Unpin()
    {
      // no-op
    }

    public void Resize(int newLength)
    {
      throw new NotSupportedException("Unable to resize mapped buffers!");
    }

    protected override void Dispose(bool disposing)
    {
      if (!isDisposed)
      {
        accessor.SafeMemoryMappedViewHandle.ReleasePointer();

        accessor.Dispose();
        file.Dispose();

        isDisposed = true;
      }
    }

    [Conditional("DEBUG")]
    private void CheckNotDisposed()
    {
      if (isDisposed)
      {
        throw new ObjectDisposedException(nameof(MappedBuffer<T>));
      }
    }

    Memory<T> IBuffer<T>.Memory => base.Memory;
  }
}
