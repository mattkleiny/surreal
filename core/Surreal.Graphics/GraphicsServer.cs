using Surreal.Graphics.Rendering;

namespace Surreal.Graphics;

/// <summary>
/// The graphics server is the main entry point for the graphics subsystem.
/// </summary>
public interface IGraphicsServer : IDisposable
{
  /// <summary>
  /// The underlying graphics backend.
  /// </summary>
  IGraphicsBackend Backend { get; }
}

/// <summary>
/// The default <see cref="IGraphicsServer"/> implementation.
/// </summary>
public sealed class GraphicsServer : IGraphicsServer
{
  /// <summary>
  /// Creates a new <see cref="GraphicsServer"/> with a <see cref="HeadlessGraphicsBackend"/>.
  /// </summary>
  public static GraphicsServer CreateHeadless()
  {
    return new GraphicsServer { Backend = new HeadlessGraphicsBackend() };
  }

  /// <summary>
  /// The underlying graphics backend.
  /// </summary>
  public required IGraphicsBackend Backend { get; init; }

  /// <summary>
  /// The per-scene render pipeline.
  /// </summary>
  public IRenderPipeline RenderPipeline { get; set; }

  public void Dispose()
  {
    if (Backend is IDisposable disposable)
    {
      disposable.Dispose();
    }
  }
}
