using Surreal.Graphics.Materials;
using Surreal.Graphics.Rendering;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// A <see cref="RenderContext"/> for <see cref="SpriteBatch"/>es.
/// </summary>
public sealed class SpriteContext(IGraphicsBackend backend) : RenderContext
{
  /// <summary>
  /// The <see cref="SpriteBatch"/> used by this context.
  /// </summary>
  public SpriteBatch Batch { get; } = new(backend);

  /// <summary>
  /// The property to use for the projection view matrix.
  /// <para/>
  /// If not specified, the identity matrix will be used.
  /// </summary>
  public MaterialProperty<Matrix4x4>? TransformProperty { get; set; }

  /// <summary>
  /// The material used by this context.
  /// </summary>
  public Material Material { get; init; } = new(backend, ShaderProgram.LoadDefaultCanvasShader(backend))
  {
    BlendState = BlendState.OneMinusSourceAlpha
  };

  public override void OnBeginPass(in RenderFrame frame, IRenderViewport viewport)
  {
    base.OnBeginPass(in frame, viewport);

    if (TransformProperty != null)
    {
      Material.Properties.SetProperty(TransformProperty.Value, viewport.ProjectionView);
    }

    Batch.Material = Material;
    Batch.Reset();
  }

  public override void OnEndPass(in RenderFrame frame, IRenderViewport viewport)
  {
    base.OnEndPass(in frame, viewport);

    Batch.Flush();
  }

  public override void Dispose()
  {
    Batch.Dispose();
    Material.Dispose();

    base.Dispose();
  }
}
