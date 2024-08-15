namespace Surreal.Graphics;

/// <summary>
/// The graphics mode to use.
/// </summary>
public enum GraphicsMode
{
  /// <summary>
  /// A mode that works on all platforms.
  /// </summary>
  Universal,
  /// <summary>
  /// A mode that enables high-definition graphics.
  /// </summary>
  HighDefinition,
}

/// <summary>
/// An abstraction over the different types of graphics backends available.
/// </summary>
public interface IGraphicsBackend
{
  static IGraphicsBackend Null { get; } = new NullGraphicsBackend();

  /// <summary>
  /// Creates a new <see cref="IGraphicsDevice"/>.
  /// </summary>
  IGraphicsDevice CreateDevice(GraphicsMode mode);

  /// <summary>
  /// A no-op <see cref="IGraphicsBackend" /> for headless environments and testing.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullGraphicsBackend : IGraphicsBackend
  {
    public IGraphicsDevice CreateDevice(GraphicsMode mode)
    {
      return IGraphicsDevice.Null;
    }
  }
}
