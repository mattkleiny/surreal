using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.Memory;

/// <summary>Represents a buffer of data of <see cref="T"/>.</summary>
public interface IBuffer<T>
{
  Span<T> Data { get; }
}

/// <summary>A <see cref="IBuffer{T}"/> that can be deterministically disposed.</summary>
public interface IDisposableBuffer<T> : IBuffer<T>, IDisposable
{
}

/// <summary>Static factories for <see cref="IBuffer{T}"/>s.</summary>
public static class Buffers
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IBuffer<T> Allocate<T>(int length)
  {
    return new ManagedBuffer<T>(length);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IBuffer<T> AllocatePinned<T>(int length)
  {
    return new PinnedBuffer<T>(length);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IDisposableBuffer<T> AllocateNative<T>(int length, bool zeroFill = false)
  {
    return new NativeBuffer<T>(length, zeroFill);
  }

  #region Buffer Implementations

  private sealed class ManagedBuffer<T> : IBuffer<T>
  {
    private readonly T[] elements;

    public ManagedBuffer(int length)
    {
      elements = new T[length];
    }

    public Span<T> Data => elements;
  }

  private sealed class PinnedBuffer<T> : IBuffer<T>
  {
    private readonly T[] elements;

    public PinnedBuffer(int length)
    {
      elements = GC.AllocateArray<T>(length, pinned: true);
    }

    public Span<T> Data => new(elements);
  }

  private sealed unsafe class NativeBuffer<T> : IDisposableBuffer<T>
  {
    private readonly int   length;
    private readonly void* address;

    private bool isDisposed;

    public NativeBuffer(int length, bool zeroFill)
    {
      this.length = length;

      address = NativeMemory.Alloc((nuint) length, (nuint) Unsafe.SizeOf<T>());

      if (zeroFill)
      {
        Data.Fill(default!);
      }
    }

    public Span<T> Data
    {
      get
      {
        CheckNotDisposed();

        return new Span<T>(address, length);
      }
    }

    public void Dispose()
    {
      CheckNotDisposed();

      NativeMemory.Free(address);
      isDisposed = true;
    }

    [Conditional("DEBUG")]
    private void CheckNotDisposed()
    {
      if (isDisposed)
      {
        throw new ObjectDisposedException(nameof(NativeBuffer<T>));
      }
    }
  }

  #endregion
}