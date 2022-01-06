﻿using System.Runtime.CompilerServices;
using Surreal.Graphics.Meshes;
using Surreal.Memory;

namespace Surreal.Graphics.Utilities;

/// <summary>An in-memory <see cref="GraphicsBuffer{T}"/> implementation, for testing purposes.</summary>
internal sealed class InMemoryGraphicsBuffer<T> : GraphicsBuffer<T>
  where T : unmanaged
{
  private static readonly int Stride = Unsafe.SizeOf<T>();

  private T[] buffer = Array.Empty<T>();

  public override Memory<T> Read(Optional<Range> range = default)
  {
    return buffer[range.GetOrDefault(Range.All)];
  }

  public override void Write(ReadOnlySpan<T> data)
  {
    var bytes = data.Length * Stride;

    if (data.Length > buffer.Length)
    {
      Array.Resize(ref buffer, data.Length);
    }

    data.CopyTo(buffer);

    Length = data.Length;
    Size   = new Size(bytes);
  }
}