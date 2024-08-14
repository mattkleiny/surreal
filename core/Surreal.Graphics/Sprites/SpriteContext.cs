using Surreal.Graphics.Materials;
using Surreal.Graphics.Rendering;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// A <see cref="RenderContext"/> for <see cref="SpriteBatch"/>es.
/// </summary>
public sealed class SpriteContext(IGraphicsDevice device) : RenderContext
{
  /// <summary>
  /// The <see cref="SpriteBatch"/> used by this context.
  /// </summary>
  public SpriteBatch Batch { get; } = new(device);

  /// <summary>
  /// The property to use for the projection view matrix.
  /// <para/>
  /// If not specified, the identity matrix will be used.
  /// </summary>
  public UniformProperty<Matrix4x4>? TransformProperty { get; set; }

  /// <summary>
  /// The material used by this context.
  /// </summary>
  public Material Material { get; init; } = new(device, ShaderProgram.LoadDefaultCanvasShader(device))
  {
    BlendState = BlendState.OneMinusSourceAlpha
  };

  public override void OnBeginPass(in RenderFrame frame, IRenderViewport viewport)
  {
    base.OnBeginPass(in frame, viewport);

    if (TransformProperty != null)
    {
      Material.Uniforms.Set(TransformProperty.Value, viewport.ProjectionView);
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
