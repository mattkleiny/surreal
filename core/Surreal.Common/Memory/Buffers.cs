﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.Memory;

/// <summary>Represents a buffer of data of <see cref="T"/>.</summary>
public interface IBuffer<T>
{
  Span<T> Span { get; }
}

/// <summary>A <see cref="IBuffer{T}"/> that can be deterministically disposed.</summary>
public interface IDisposableBuffer<T> : IBuffer<T>, IDisposable
{
}

/// <summary>Static factories for <see cref="IBuffer{T}"/>s.</summary>
public static class Buffers
{
  public static IBuffer<T> Allocate<T>(int length)
  {
    return new ManagedBuffer<T>(length);
  }

  public static IBuffer<T> AllocatePinned<T>(int length, bool zeroFill = false)
    where T : unmanaged
  {
    return new PinnedBuffer<T>(length, zeroFill);
  }

  public static IDisposableBuffer<T> AllocateNative<T>(int length, bool zeroFill = false)
    where T : unmanaged
  {
    return new NativeBuffer<T>(length, zeroFill);
  }

  /// <summary>A buffer backed by a managed array.</summary>
  private sealed class ManagedBuffer<T> : IBuffer<T>
  {
    private readonly T[] elements;

    public ManagedBuffer(int length)
    {
      elements = new T[length];
    }

    public Span<T> Span => elements;
  }

  /// <summary>A buffer backed by a pinned array.</summary>
  private sealed class PinnedBuffer<T> : IBuffer<T>
    where T : unmanaged
  {
    private readonly T[] elements;

    public PinnedBuffer(int length, bool zeroFill)
    {
      elements = zeroFill
        ? GC.AllocateArray<T>(length, pinned: true)
        : GC.AllocateUninitializedArray<T>(length, pinned: true);
    }

    public Span<T> Span => new(elements);
  }

  /// <summary>A buffer backed by native memory.</summary>
  private sealed unsafe class NativeBuffer<T> : IDisposableBuffer<T>
    where T : unmanaged
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
        Span.Fill(default!);
      }
    }

    ~NativeBuffer()
    {
      Dispose();
    }

    public Span<T> Span
    {
      get
      {
        CheckNotDisposed();

        return new Span<T>(address, length);
      }
    }

    public void Dispose()
    {
      if (!isDisposed)
      {
        NativeMemory.Free(address);
        GC.SuppressFinalize(this);

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
  }
}