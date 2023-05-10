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

  /// <summary>
  /// The active <see cref="IRenderPipeline"/>.
  /// </summary>
  IRenderPipeline? Pipeline { get; set; }
}

/// <summary>
/// The default <see cref="IGraphicsServer"/> implementation.
/// </summary>
public sealed class GraphicsServer : IGraphicsServer
{
  private IRenderPipeline? _pipeline;

  /// <summary>
  /// Creates a new <see cref="GraphicsServer"/> with a <see cref="HeadlessGraphicsBackend"/>.
  /// </summary>
  public static GraphicsServer CreateHeadless() => new()
  {
    Backend = new HeadlessGraphicsBackend()
  };

  public required IGraphicsBackend Backend { get; init; }

  public IRenderPipeline? Pipeline
  {
    get => _pipeline;
    set
    {
      if (_pipeline != value)
      {
        _pipeline?.Dispose();
        _pipeline = value;
        _pipeline?.Initialize();
      }
    }
  }

  public void Dispose()
  {
    Pipeline?.Dispose();
    Backend.Dispose();
  }
}
