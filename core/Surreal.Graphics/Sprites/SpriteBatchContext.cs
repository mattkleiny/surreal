using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Shaders;
using Surreal.Resources;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// A <see cref="RenderContext"/> for sprite rendering.
/// </summary>
public sealed class SpriteContext : RenderContext
{
  public SpriteContext(IGraphicsServer server, Material material)
  {
    Material = material;
    SpriteBatch = new SpriteBatch(server);
  }

  /// <summary>
  /// The <see cref="Material"/> used for rendering.
  /// </summary>
  public Material Material { get; }

  /// <summary>
  /// The <see cref="SpriteBatch"/> used by this context.
  /// </summary>
  public SpriteBatch SpriteBatch { get; }

  public override void OnBeginFrame(in RenderFrame frame)
  {
    base.OnBeginFrame(in frame);

    SpriteBatch.Begin(Material);
  }

  public override void OnEndFrame(in RenderFrame frame)
  {
    SpriteBatch.Flush();

    base.OnEndFrame(in frame);
  }

  public override void Dispose()
  {
    SpriteBatch.Dispose();
    Material.Dispose();

    base.Dispose();
  }
}

/// <summary>
/// A <see cref="IRenderContextDescriptor"/> for <see cref="SpriteContext"/>.
/// </summary>
public sealed class SpriteContextDescriptor : IRenderContextDescriptor
{
  /// <summary>
  /// The initial projection-view matrix to use.
  /// </summary>
  public Matrix4x4 ProjectionView { get; init; } = Matrix4x4.Identity;

  /// <summary>
  /// The custom <see cref="Material"/> to use for rendering.
  /// </summary>
  public Optional<Material> Material { get; init; }

  /// <summary>
  /// The <see cref="ColorPalette"/> to use for rendering.
  /// </summary>
  public Optional<ColorPalette> ColorPalette { get; init; }

  public async Task<IRenderContext> BuildContextAsync(IGraphicsServer server, IResourceManager resources, CancellationToken cancellationToken)
  {
    var material = await ResolveMaterialAsync(server, resources, cancellationToken);

    return new SpriteContext(server, material);
  }

  private async Task<Material> ResolveMaterialAsync(IGraphicsServer server, IResourceManager resources, CancellationToken cancellationToken)
  {
    if (Material.IsNone)
    {
      return await resources.LoadDefaultSpriteMaterialAsync(cancellationToken);
    }

    return Material.Value;
  }
}
