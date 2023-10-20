namespace Surreal.Graphics;

/// <summary>
/// The graphics context is the top-level entry point for the graphics subsystem.
/// </summary>
public interface IGraphicsContext
{
  /// <summary>
  /// The underlying graphics backend.
  /// </summary>
  IGraphicsBackend Backend { get; }
}

/// <summary>
/// The default <see cref="IGraphicsContext"/> implementation.
/// </summary>
public sealed record GraphicsContext(IGraphicsBackend Backend) : IGraphicsContext, IDisposable
{
  /// <summary>
  /// Creates a new <see cref="GraphicsContext"/> with a <see cref="HeadlessGraphicsBackend"/>.
  /// </summary>
  public static GraphicsContext CreateHeadless()
  {
    return new GraphicsContext(new HeadlessGraphicsBackend());
  }

  public void Dispose()
  {
  }
}
