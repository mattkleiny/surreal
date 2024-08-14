using Surreal.Collections.Grids;
using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Memory;

namespace Surreal.Graphics.Utilities;

/// <summary>
/// A utility for rendering pixels to the screen.
/// </summary>
public class PixelCanvas : IDisposable
{
  private readonly DenseGrid<Color32> _pixels;
  private readonly Material _material;
  private readonly Texture _texture;
  private readonly Mesh<Vertex2> _mesh;

  public PixelCanvas(IGraphicsDevice device, int width, int height)
  {
    _mesh = Mesh.CreateQuad(device);

    _pixels = new DenseGrid<Color32>(width, height, Color32.Clear);
    _texture = new Texture(device, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);
    _material = new Material(device, ShaderProgram.LoadDefaultCanvasShader(device))
    {
      BlendState = BlendState.OneMinusSourceAlpha,
      Uniforms =
      {
        { "u_texture", _texture }
      }
    };
  }

  /// <summary>
  /// The pixels in the canvas.
  /// </summary>
  public SpanGrid<Color32> Pixels => _pixels.Span;

  /// <summary>
  /// Draws the canvas to the screen as a fullscreen quad.
  /// </summary>
  public void DrawQuad()
  {
    _texture.WritePixels<Color32>(_pixels.Width, _pixels.Height, _pixels.Span);

    _mesh.Draw(_material);
  }

  public void Dispose()
  {
    _material.Dispose();
    _texture.Dispose();
    _mesh.Dispose();
  }
}
