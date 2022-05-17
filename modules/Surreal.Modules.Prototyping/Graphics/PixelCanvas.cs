using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics;

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
    texture = new Texture(server, TextureFormat.Rgba8);
    mesh    = Mesh.CreateQuad(server);
  }

  public int Width  => pixels.Width;
  public int Height => pixels.Height;

  public SpanGrid<Color32>         Pixels          => pixels.Span;
  public MaterialProperty<Texture> TextureProperty { get; set; } = Material.DefaultTexture;

  public void Draw(Material material)
  {
    texture.WritePixels<Color32>(Width, Height, Pixels);

    material.Properties.Set(TextureProperty, texture);

    mesh.Draw(material);
  }

  public void Draw(SpriteBatch batch)
  {
    texture.WritePixels<Color32>(Width, Height, Pixels);

    batch.Draw(texture, Vector2.Zero, new Vector2(2f, 2f)); // TODO: why doubled?
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
