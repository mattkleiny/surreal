using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.Memory
{
  public interface IBuffer<T>
  {
    Span<T> Data { get; }
  }

  public interface IDisposableBuffer<T> : IBuffer<T>, IDisposable
  {
  }

  public static class Buffers
  {
    public static IBuffer<T> Allocate<T>(int length)
      => new ManagedBuffer<T>(length);

    public static IBuffer<T> AllocatePinned<T>(int length)
      => new PinnedBuffer<T>(length);

    public static IDisposableBuffer<T> AllocateNative<T>(int length, bool zeroFill = false)
      => new NativeBuffer<T>(length, zeroFill);

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

    private sealed class NativeBuffer<T> : IDisposableBuffer<T>
    {
      private readonly int    length;
      private readonly IntPtr address;

      private bool isDisposed;

      public NativeBuffer(int length, bool zeroFill)
      {
        this.length = length;

        // TODO: replace with NativeMemory.Allocate() once it's available
        address = Marshal.AllocHGlobal(length * Unsafe.SizeOf<T>());

        if (zeroFill)
        {
          Data.Fill(default!);
        }
      }

      public unsafe Span<T> Data
      {
        get
        {
          CheckNotDisposed();
          return new Span<T>(address.ToPointer(), length);
        }
      }

      public void Dispose()
      {
        CheckNotDisposed();

        Marshal.FreeHGlobal(address);
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
}
