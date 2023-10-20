using System.Buffers;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Surreal.IO;

namespace Surreal.Memory;

/// <summary>
/// Represents a buffer of data of <see cref="T" />.
/// </summary>
public interface IBuffer<T>
{
  /// <summary>
  /// The underlying <see cref="Memory{T}" /> representing the buffer data.
  /// </summary>
  Memory<T> Memory { get; }

  /// <summary>
  /// The underlying <see cref="Span{T}" /> representing the buffer data.
  /// </summary>
  Span<T> Span { get; }

  /// <summary>
  /// Resizes the underlying buffer storage.
  /// </summary>
  void Resize(int newLength);
}

/// <summary>
/// A <see cref="IBuffer{T}" /> that can be deterministically disposed.
/// </summary>
public interface IDisposableBuffer<T> : IBuffer<T>, IDisposable
{
}

/// <summary>
/// Static factories for <see cref="IBuffer{T}" />s.
/// </summary>
public static class Buffers
{
  public static IBuffer<T> Allocate<T>(int length)
  {
    return new ManagedBuffer<T>(length);
  }

  public static IBuffer<T> AllocatePinned<T>(int length, bool zeroFill = false)
  {
    return new PinnedBuffer<T>(length, zeroFill);
  }

  public static IDisposableBuffer<T> AllocateNative<T>(int length, bool zeroFill = false)
    where T : unmanaged
  {
    return new NativeBuffer<T>(length, zeroFill);
  }

  public static IDisposableBuffer<T> AllocateMapped<T>(VirtualPath path, int offset, int length)
    where T : unmanaged
  {
    return new MappedBuffer<T>(path, offset, length);
  }

  /// <summary>
  /// A buffer backed by a managed array.
  /// </summary>
  private sealed class ManagedBuffer<T>(int length) : IBuffer<T>
  {
    private T[] _elements = new T[length];

    public Memory<T> Memory => _elements;
    public Span<T> Span => _elements;

    public void Resize(int newLength)
    {
      Array.Resize(ref _elements, newLength);
    }
  }

  /// <summary>
  /// A buffer backed by a pinned array.
  /// </summary>
  private sealed class PinnedBuffer<T>(int length, bool zeroFill) : IBuffer<T>
  {
    private readonly T[] _elements = zeroFill
      ? GC.AllocateArray<T>(length, true)
      : GC.AllocateUninitializedArray<T>(length, true);

    public Memory<T> Memory => _elements;
    public Span<T> Span => _elements;

    public void Resize(int newLength)
    {
      throw new NotSupportedException("Unable to resize pinned buffer!");
    }
  }

  /// <summary>
  /// A buffer backed by native memory.
  /// </summary>
  [SuppressMessage("Reliability", "CA2015:Do not define finalizers for types derived from MemoryManager<T>")]
  private sealed unsafe class NativeBuffer<T>(int length, bool zeroFill) : MemoryManager<T>, IDisposableBuffer<T>
    where T : unmanaged
  {
    private void* _buffer = zeroFill
      ? NativeMemory.AllocZeroed((nuint)length, (nuint)Unsafe.SizeOf<T>())
      : NativeMemory.Alloc((nuint)length, (nuint)Unsafe.SizeOf<T>());

    private bool _isDisposed;

    ~NativeBuffer()
    {
      Dispose(false);
    }

    public Span<T> Span => GetSpan();

    Memory<T> IBuffer<T>.Memory => base.Memory;

    public void Resize(int newLength)
    {
      _buffer = NativeMemory.Realloc(_buffer, (nuint)newLength);
    }

    public override Span<T> GetSpan()
    {
      ObjectDisposedException.ThrowIf(_isDisposed, this);

      return new Span<T>(_buffer, length);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
      return default; // no-op
    }

    public override void Unpin()
    {
      // no-op
    }

    protected override void Dispose(bool disposing)
    {
      if (!_isDisposed)
      {
        NativeMemory.Free(_buffer);

        _isDisposed = true;
      }
    }
  }

  /// <summary>
  /// A <see cref="IDisposableBuffer{T}" /> for memory mapped files.
  /// </summary>
  private sealed unsafe class MappedBuffer<T> : MemoryManager<T>, IDisposableBuffer<T>
    where T : unmanaged
  {
    private readonly MemoryMappedViewAccessor _accessor;
    private readonly MemoryMappedFile _file;
    private readonly byte* _pointer;

    private bool _isDisposed;

    public MappedBuffer(VirtualPath path, int offset, int length)
    {
      if (!path.SupportsMemoryMapping())
      {
        throw new InvalidOperationException($"The given path does not support memory mapped files: {path}");
      }

      _file = path.OpenMemoryMappedFile(offset, length);
      _accessor = _file.CreateViewAccessor(offset, length, MemoryMappedFileAccess.ReadWrite);

      _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref _pointer);
    }

    public Span<T> Span => GetSpan();

    public void Resize(int newLength)
    {
      throw new NotSupportedException("Unable to resize mapped buffers!");
    }

    Memory<T> IBuffer<T>.Memory => base.Memory;

    public override Span<T> GetSpan()
    {
      ObjectDisposedException.ThrowIf(_isDisposed, this);

      return new Span<T>(_pointer, (int)_accessor.Capacity);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
      return default; // no-op
    }

    public override void Unpin()
    {
      // no-op
    }

    protected override void Dispose(bool disposing)
    {
      if (!_isDisposed)
      {
        _accessor.SafeMemoryMappedViewHandle.ReleasePointer();

        _accessor.Dispose();
        _file.Dispose();

        _isDisposed = true;
      }
    }
  }
}
