namespace Surreal.Graphics;

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IGraphicsContext" /> implementation.
/// </summary>
public readonly record struct GraphicsHandle(nint Id)
{
  public static GraphicsHandle None => default;
  public static GraphicsHandle Invalid => new(-1);

  public GraphicsHandle(uint Id)
    : this((nint)Id)
  {
  }

  public GraphicsHandle(int Id)
    : this((nint)Id)
  {
  }

  public static implicit operator nint(GraphicsHandle handle) => handle.Id;
  public static implicit operator int(GraphicsHandle handle) => (int)handle.Id;
  public static implicit operator uint(GraphicsHandle handle) => (uint)handle.Id;
}
