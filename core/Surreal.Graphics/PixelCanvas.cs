using Surreal.Collections;
using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics;

/// <summary>
/// A utility for rendering pixels to the screen.
/// </summary>
public sealed class PixelCanvas : IDisposable
{
  private static MaterialProperty<Texture> Texture { get; } = new("u_texture");

  private readonly DenseGrid<Color32> _pixels;
  private readonly Material _material;
  private readonly Texture _texture;
  private readonly Mesh<Vertex2> _mesh;

  public PixelCanvas(IGraphicsBackend backend, int width, int height)
  {
    _pixels = new DenseGrid<Color32>(width, height);

    _mesh = Mesh.CreateQuad(backend);
    _texture = new Texture(backend, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);
    _material = new Material(backend, ShaderProgram.LoadDefaultCanvasShader(backend))
    {
      BlendState = BlendState.OneMinusSourceAlpha
    };

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
    _mesh.Draw(_material);
  }

  public void Dispose()
  {
    _material.Dispose();
    _texture.Dispose();
    _mesh.Dispose();
  }
}
