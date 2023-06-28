namespace Surreal.Graphics;

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IGraphicsContext" /> implementation.
/// </summary>
public readonly record struct GraphicsHandle(nint Id)
{
  public static GraphicsHandle None => default;

  public static implicit operator nint(GraphicsHandle handle)
  {
    return handle.Id;
  }

  public static implicit operator int(GraphicsHandle handle)
  {
    return (int)handle.Id;
  }

  public static implicit operator uint(GraphicsHandle handle)
  {
    return (uint)handle.Id;
  }
}
