using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Rendering;

namespace Surreal.Scenes.UI;

/// <summary>
/// A rendering context for a canvas.
/// </summary>
public sealed class WidgetContext(IGraphicsBackend backend) : RenderContext
{
  /// <summary>
  /// The main canvas batch.
  /// </summary>
  public WidgetBatch WidgetBatch { get; } = new(backend);

  protected override void Dispose()
  {
    WidgetBatch.Dispose();

    base.Dispose();
  }
}

/// <summary>
/// A batched mesh of culled quads, for rendering UI elements.
/// </summary>
public sealed class WidgetBatch(IGraphicsBackend backend) : IDisposable
{
  private readonly Material _material = new(backend, ShaderProgram.LoadDefaultCanvasShader(backend));

  public void Dispose()
  {
    _material.Dispose();
  }
}
