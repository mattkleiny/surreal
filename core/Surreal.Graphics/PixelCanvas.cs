using Surreal.Collections;
using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Memory;

namespace Surreal.Graphics;

/// <summary>
/// A utility for rendering pixels to the screen.
/// </summary>
public class PixelCanvas : IDisposable
{
  private readonly DenseGrid<Color32> _pixels;
  private readonly Material _material;
  private readonly Texture _texture;
  private readonly Mesh<Vertex2> _mesh;

  public PixelCanvas(IGraphicsBackend backend, int width, int height)
  {
    _pixels = new DenseGrid<Color32>(width, height, Color32.Clear);

    _mesh = Mesh.CreateQuad(backend);
    _texture = new Texture(backend, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);
    _material = new Material(backend, ShaderProgram.LoadDefaultCanvasShader(backend))
    {
      BlendState = BlendState.OneMinusSourceAlpha
    };

    _material.Properties.SetProperty("u_texture", _texture);
  }

  /// <summary>
  /// The pixels in the canvas.
  /// </summary>
  public SpanGrid<Color32> Pixels => _pixels.AsSpanGrid();

  /// <summary>
  /// Draws the canvas to the screen as a fullscreen quad.
  /// </summary>
  public void DrawQuad()
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
