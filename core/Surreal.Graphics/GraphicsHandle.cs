using Surreal.Collections;

namespace Surreal.Graphics;

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IGraphicsBackend" /> implementation.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct GraphicsHandle(ulong Id)
{
  public static GraphicsHandle None => default;

  public static GraphicsHandle FromInt(int index) => new((ulong)index);
  public static GraphicsHandle FromUInt(uint index) => new(index);
  public static GraphicsHandle FromNInt(nint index) => new((ulong)index);
  public static GraphicsHandle FromULong(ulong index) => new(index);
  public static GraphicsHandle FromArenaIndex(ArenaIndex index) => new(index);

  public static implicit operator int(GraphicsHandle handle) => (int)handle.Id;
  public static implicit operator uint(GraphicsHandle handle) => (uint)handle.Id;
  public static implicit operator nint(GraphicsHandle handle) => (nint)handle.Id;
  public static implicit operator ulong(GraphicsHandle handle) => handle.Id;
  public static implicit operator ArenaIndex(GraphicsHandle handle) => ArenaIndex.FromUlong(handle);
}
