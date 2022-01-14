﻿using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

// TODO: implement IBuffer<T> somehow?

/// <summary>A buffer of data on the <see cref="IGraphicsDevice"/>.</summary>
public abstract class GraphicsBuffer : GraphicsResource, IHasSizeEstimate
{
  public int  Length { get; protected set; }
  public Size Size   { get; protected set; }
}

/// <summary>A strongly-typed <see cref="GraphicsBuffer"/> of <see cref="T"/>.</summary>
public abstract class GraphicsBuffer<T> : GraphicsBuffer
  where T : unmanaged
{
  public abstract Memory<T> Read(Optional<Range> range = default);
  public abstract void      Write(ReadOnlySpan<T> buffer);
}
