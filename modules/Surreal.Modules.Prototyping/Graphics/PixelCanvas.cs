using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics;

/// <summary>
///   A fun canvas of managed RGBA pixels that can be blitted to the screen.
///   Use this to build per-pixel simulations.
/// </summary>
public class PixelCanvas : IDisposable
{
  private readonly Grid<Color32> _pixels;
  private readonly Texture _texture;

  public PixelCanvas(IGraphicsServer server, int width, int height)
  {
    _pixels = new Grid<Color32>(width, height);
    _texture = new Texture(server);
  }

  public int Width => _pixels.Width;
  public int Height => _pixels.Height;

  public SpanGrid<Color32> Pixels => _pixels.Span;

  public void Dispose()
  {
    _texture.Dispose();
  }

  public void Draw(SpriteBatch batch)
  {
    _texture.WritePixels<Color32>(Width, Height, Pixels);

    batch.Draw(_texture, Vector2.Zero);
  }

  public void DrawNormalized(SpriteBatch batch)
  {
    _texture.WritePixels<Color32>(Width, Height, Pixels);

    batch.Draw(_texture, Vector2.Zero, new Vector2(2f, 2f));
  }

  public void Fill(Color32 value)
  {
    _pixels.Fill(value);
  }
}


