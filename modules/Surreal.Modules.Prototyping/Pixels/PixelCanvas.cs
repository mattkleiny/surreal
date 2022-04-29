using Surreal.Graphics;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Pixels;

/// <summary>
/// A fun canvas of managed RGBA pixels that can be blitted to the screen.
/// Use this to build per-pixel simulations.
/// </summary>
public class PixelCanvas : IDisposable
{
  private readonly Grid<Color32> pixels;
  private readonly Texture texture;
  private readonly Mesh mesh;

  public PixelCanvas(IGraphicsServer server, int width, int height)
  {
    pixels  = new Grid<Color32>(width, height);
    texture = new Texture(server, TextureFormat.Rgba8888);
    mesh    = Mesh.CreateQuad(server);
  }

  public int Width  => pixels.Width;
  public int Height => pixels.Height;

  public SpanGrid<Color32> Pixels => pixels.Span;

  public void Draw(ShaderProgram shader)
  {
    texture.WritePixels<Color32>(Width, Height, Pixels);

    shader.SetUniform("u_projectionView", Matrix4x4.Identity);
    shader.SetUniform("u_texture", texture, 0);

    mesh.Draw(shader);
  }

  public void Fill(Color32 value)
  {
    pixels.Fill(value);
  }

  public void Dispose()
  {
    mesh.Dispose();
    texture.Dispose();
  }
}
