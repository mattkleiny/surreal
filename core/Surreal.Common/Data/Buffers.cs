using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.Data {
  public interface IBuffer<T>
      where T : unmanaged {
    int     Length { get; }
    int     Stride { get; }
    Size    Size   { get; }
    Span<T> Data   { get; }

    void Clear();
    void Fill(T value);

    IBuffer<T> Slice(int offset, int size);
  }

  public interface IDisposableBuffer<T> : IBuffer<T>, IDisposable
      where T : unmanaged {
  }

  public interface IResizableBuffer<T> : IBuffer<T>
      where T : unmanaged {
    void Resize(int newLength);
  }

  public static class Buffers {
    public static IBuffer<T> Allocate<T>(int count)
        where T : unmanaged => new ManagedBuffer<T>(count);

    public static IBuffer<T> AllocatePinned<T>(int count)
        where T : unmanaged => new PinnedBuffer<T>(count);

    public static IDisposableBuffer<T> AllocateNative<T>(int count, bool zeroFill = false)
        where T : unmanaged => new UnmanagedBuffer<T>(count, zeroFill);

    public static IDisposableBuffer<T> AllocateMapped<T>(string path, int offset, int length)
        where T : unmanaged => new MemoryMappedBuffer<T>(path, offset, length);

    private abstract class Buffer<T> : IBuffer<T>
        where T : unmanaged {
      protected Buffer(int count) {
        Length = count;
      }

      public int  Length { get; protected set; }
      public int  Stride => Unsafe.SizeOf<T>();
      public Size Size   => new(Length * Stride);

      public abstract Span<T> Data { get; }

      public void Clear()       => Fill(default);
      public void Fill(T value) => Data.Fill(value);

      public virtual IBuffer<T> Slice(int offset, int size) => new BufferSlice(this, offset, size);

      [DebuggerDisplay("{Size} sliced from ({source})")]
      private sealed class BufferSlice : Buffer<T> {
        private readonly IBuffer<T> source;
        private readonly int        offset;

        public BufferSlice(IBuffer<T> source, int offset, int count)
            : base(count) {
          this.source = source;
          this.offset = offset;
        }

        public override Span<T> Data => source.Data.Slice(offset, Length);
      }
    }

    [DebuggerDisplay("{Size} allocated on-heap")]
    private sealed class ManagedBuffer<T> : Buffer<T>, IResizableBuffer<T>
        where T : unmanaged {
      private T[] elements;

      public ManagedBuffer(int count)
          : base(count) {
        elements = new T[count];
      }

      public override Span<T> Data => new(elements);

      public void Resize(int newLength) {
        Array.Resize(ref elements, newLength);
        Length = newLength;
      }
    }

    [DebuggerDisplay("{Size} allocated on-heap (pinned)")]
    private sealed class PinnedBuffer<T> : Buffer<T>, IResizableBuffer<T>
        where T : unmanaged {
      private T[] elements;

      public PinnedBuffer(int count)
          : base(count) {
        elements = GC.AllocateArray<T>(count, pinned: true);
      }

      public override Span<T> Data => new(elements);

      public void Resize(int newLength) {
        Array.Resize(ref elements, newLength);
        Length = newLength;
      }
    }

    [DebuggerDisplay("{Size} allocated off-heap")]
    private sealed class UnmanagedBuffer<T> : Buffer<T>, IResizableBuffer<T>, IDisposableBuffer<T>
        where T : unmanaged {
      private IntPtr address;
      private bool   disposed;

      public UnmanagedBuffer(int count, bool zeroFill)
          : base(count) {
        address = Marshal.AllocHGlobal(count * Stride);

        if (zeroFill) {
          Clear();
        }
      }

      public override unsafe Span<T> Data {
        get {
          CheckNotDisposed();

          return new Span<T>(address.ToPointer(), Length);
        }
      }

      public void Resize(int newLength) {
        CheckNotDisposed();

        address = Marshal.ReAllocHGlobal(address, new IntPtr(newLength));
        Length  = newLength;
      }

      public void Dispose() {
        if (!disposed) {
          Marshal.FreeHGlobal(address);
          disposed = true;
        }
      }

      [Conditional("DEBUG")]
      private void CheckNotDisposed() {
        if (disposed) {
          throw new ObjectDisposedException(nameof(UnmanagedBuffer<T>));
        }
      }
    }

    [DebuggerDisplay("{Size} mapped from {path}")]
    private sealed unsafe class MemoryMappedBuffer<T> : Buffer<T>, IDisposableBuffer<T>
        where T : unmanaged {
// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
      private readonly string                   path;
      private readonly MemoryMappedFile         file;
      private readonly MemoryMappedViewAccessor accessor;
      private readonly byte*                    pointer;
      private          bool                     disposed;

      public MemoryMappedBuffer(string path, int offset, int length)
          : base(length / sizeof(T)) {
        this.path = path;

        file = MemoryMappedFile.CreateFromFile(
            path: path,
            mode: FileMode.OpenOrCreate,
            mapName: Guid.NewGuid().ToString(), // needs to be unique
            capacity: length,
            access: MemoryMappedFileAccess.ReadWrite
        );

        accessor = file.CreateViewAccessor(offset, length);
        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
      }

      public override Span<T> Data {
        get {
          CheckNotDisposed();

          return new Span<T>(pointer, (int) accessor.Capacity);
        }
      }

      public void Dispose() {
        if (disposed) {
          accessor.SafeMemoryMappedViewHandle.ReleasePointer();
          accessor.Dispose();

          file.Dispose();

          disposed = true;
        }
      }

      [Conditional("DEBUG")]
      private void CheckNotDisposed() {
        if (disposed) {
          throw new ObjectDisposedException(nameof(UnmanagedBuffer<T>));
        }
      }
    }
  }
}