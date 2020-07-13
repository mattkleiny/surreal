using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Surreal.Memory {
  public ref struct ArenaAllocator {
    private readonly Span<byte> buffer;
    private volatile int        position;

    public ArenaAllocator(Span<byte> buffer) {
      this.buffer = buffer;

      position = 0;
    }

    public Size Capacity  => new Size(buffer.Length);
    public Size Remaining => new Size(buffer.Length - position);

    public unsafe ref T Allocate<T>()
        where T : unmanaged {
      var size    = sizeof(T);
      var pointer = Unsafe.AsPointer(ref buffer[position]);

      CheckCapacity<T>(size);
      Unsafe.InitBlock(pointer, 0, (uint) size);

      position += size;

      return ref Unsafe.AsRef<T>(pointer);
    }

    public void Clear() {
      buffer.Fill(default);
      position = 0;
    }

    [Conditional("DEBUG")]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckCapacity<T>(int size) {
      if (position + size > buffer.Length) {
        throw new Exception($"Not enough space on the arena allocator to allocate {typeof(T)}. Available {Remaining} of {Capacity}.");
      }
    }
  }
}