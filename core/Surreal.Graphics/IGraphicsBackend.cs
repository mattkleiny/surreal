namespace Surreal.Graphics;

/// <summary>
/// An abstraction over the different types of graphics backends available.
/// </summary>
public interface IGraphicsBackend
{
  static IGraphicsBackend Null { get; } = new NullGraphicsBackend();

  /// <summary>
  /// Creates a new <see cref="IGraphicsDevice"/>.
  /// </summary>
  IGraphicsDevice CreateDevice();

  /// <summary>
  /// A no-op <see cref="IGraphicsBackend" /> for headless environments and testing.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullGraphicsBackend : IGraphicsBackend
  {
    public IGraphicsDevice CreateDevice()
    {
      return IGraphicsDevice.Null;
    }
  }
}
