using Surreal.Collections;
using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics;

/// <summary>
/// A utility for rendering pixels to the screen.
/// </summary>
public sealed class PixelCanvas : IDisposable
{
  private static MaterialProperty<Matrix4x4> ProjectionView { get; } = new("u_projectionView");
  private static MaterialProperty<Texture> Texture { get; } = new("u_texture");

  private readonly DenseGrid<Color32> _pixels;
  private readonly Material _material;
  private readonly Texture _texture;

  public PixelCanvas(IGraphicsBackend backend, int width, int height)
  {
    var shader = ShaderProgram.Load(backend, "resx://Surreal.Graphics/Assets/Embedded/shaders/sprite.glsl");

    _pixels = new DenseGrid<Color32>(width, height);
    _material = new Material(backend, shader)
    {
      BlendState = BlendState.OneMinusSourceAlpha
    };

    _texture = new Texture(backend, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);

    _material.Properties.SetProperty(ProjectionView, Matrix4x4.Identity);
    _material.Properties.SetProperty(Texture, _texture);
  }

  /// <summary>
  /// The width of the canvas, in pixels.
  /// </summary>
  public int Width => _pixels.Width;

  /// <summary>
  /// The height of the canvas, in pixels.
  /// </summary>
  public int Height => _pixels.Height;

  /// <summary>
  /// Read/write access to the pixels in the canvas.
  /// </summary>
  public ref Color32 this[int x, int y] => ref _pixels[x, y];

  /// <summary>
  /// Draws the canvas to the screen as a fullscreen quad.
  /// </summary>
  public void DrawFullscreenQuad()
  {
    _texture.WritePixels<Color32>(_pixels.Width, _pixels.Height, _pixels.AsSpan());
    _material.DrawFullscreenQuad();
  }

  public void Dispose()
  {
    _material.Dispose();
    _texture.Dispose();
  }
}
