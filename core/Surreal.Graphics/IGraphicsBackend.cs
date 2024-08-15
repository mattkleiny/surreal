namespace Surreal.Graphics;

/// <summary>
/// Possible graphics modes for the backend.
/// </summary>
public enum GraphicsMode
{
  /// <summary>
  /// The graphics backend should be universal and work on all platforms.
  /// </summary>
  Universal,

  /// <summary>
  /// The graphics backend should high-fidelity rendering.
  /// </summary>
  HighFidelity,
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
