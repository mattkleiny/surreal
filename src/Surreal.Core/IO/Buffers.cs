using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.IO {
  public interface IBuffer<T>
      where T : unmanaged {
    int     Count  { get; }
    int     Stride { get; }
    Size    Size   { get; }
    Span<T> Span   { get; }

    void Clear();
    void Fill(T value);

    IBuffer<T> Slice(int offset, int size);
  }

  public interface IDisposableBuffer<T> : IBuffer<T>, IDisposable
      where T : unmanaged {
  }

  public interface IBufferPool<T>
      where T : unmanaged {
    IDisposableBuffer<T> Rent(int count);
  }

  public static class Buffers {
    public static IBuffer<T> Allocate<T>(int count)
        where T : unmanaged => new HeapBuffer<T>(count);

    public static IDisposableBuffer<T> AllocateOffHeap<T>(int count)
        where T : unmanaged => new OffHeapBuffer<T>(count);

    public static IBufferPool<T> AllocatePool<T>(int maxBufferSize = 1048576, int bucketSize = 50)
        where T : unmanaged => new HeapBufferPool<T>(maxBufferSize, bucketSize);

    public static IDisposableBuffer<T> MapFromFile<T>(string path, int offset, int length)
        where T : unmanaged => new MemoryMappedBuffer<T>(path, offset, length);

    private abstract class AbstractBuffer<T> : IBuffer<T>
        where T : unmanaged {
      protected AbstractBuffer(int count) {
        Count = count;
      }

      public int  Count  { get; }
      public int  Stride => Unsafe.SizeOf<T>();
      public Size Size   => new Size(Count * Stride);

      public abstract Span<T> Span { get; }

      public void Clear()       => Fill(default);
      public void Fill(T value) => Span.Fill(value);

      public virtual IBuffer<T> Slice(int offset, int size) => new BufferSlice<T>(this, offset, size);
    }

    [DebuggerDisplay("{Size} allocated on-heap")]
    private sealed class HeapBuffer<T> : AbstractBuffer<T>
        where T : unmanaged {
      private readonly T[] elements;

      public HeapBuffer(int count)
          : base(count) {
        elements = new T[count];
      }

      public override Span<T> Span => new Span<T>(elements);
    }

    [DebuggerDisplay("{Size} allocated off-heap")]
    private sealed class OffHeapBuffer<T> : AbstractBuffer<T>, IDisposableBuffer<T>
        where T : unmanaged {
      private readonly IntPtr address;
      private          bool   disposed;

      public OffHeapBuffer(int count)
          : base(count) {
        address = Marshal.AllocHGlobal(count * Stride);
        Clear(); // zero-fill the array
      }

      public override unsafe Span<T> Span {
        get {
          CheckNotDisposed();
          return new Span<T>(address.ToPointer(), Count);
        }
      }

      public void Dispose() {
        if (!disposed) {
          Marshal.FreeHGlobal(address);
          disposed = true;
        }
      }

      [Conditional("DEBUG")]
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private void CheckNotDisposed() {
        if (disposed) {
          throw new ObjectDisposedException(nameof(OffHeapBuffer<T>));
        }
      }
    }

    [DebuggerDisplay("{Size} mapped from {path}")]
    private sealed unsafe class MemoryMappedBuffer<T> : AbstractBuffer<T>, IDisposableBuffer<T>
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

      public override Span<T> Span {
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
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private void CheckNotDisposed() {
        if (disposed) {
          throw new ObjectDisposedException(nameof(OffHeapBuffer<T>));
        }
      }
    }

    [DebuggerDisplay("{Size} sliced from ({source})")]
    private sealed class BufferSlice<T> : AbstractBuffer<T>
        where T : unmanaged {
      private readonly IBuffer<T> source;
      private readonly int        offset;

      public BufferSlice(IBuffer<T> source, int offset, int count)
          : base(count) {
        this.source = source;
        this.offset = offset;
      }

      public override Span<T> Span => source.Span.Slice(offset, Count);
    }

    private sealed class HeapBufferPool<T> : IBufferPool<T>
        where T : unmanaged {
      private readonly ArrayPool<T> pool;

      public HeapBufferPool(int maxBufferSize, int bucketSize) {
        pool = ArrayPool<T>.Create(maxBufferSize, bucketSize);
      }

      public IDisposableBuffer<T> Rent(int count) {
        return new PooledBuffer(this, pool.Rent(count));
      }

      [DebuggerDisplay("{Size} rented from {parent}")]
      private sealed class PooledBuffer : AbstractBuffer<T>, IDisposableBuffer<T> {
        private readonly HeapBufferPool<T> parent;
        private          T[]               array;

        public PooledBuffer(HeapBufferPool<T> parent, T[] array)
            : base(array.Length) {
          this.parent = parent;
          this.array  = array;
        }

        public override Span<T> Span => new Span<T>(array);

        public void Dispose() {
          parent.pool.Return(array, true);
          array = null!; // drop reference to pooled data
        }
      }
    }
  }
}