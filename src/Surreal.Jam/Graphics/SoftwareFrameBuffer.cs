using System;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics.Grids;

namespace Surreal.Graphics
{
  public sealed class SoftwareFrameBuffer : IDisposable
  {
    private readonly Pixmap   pixmap;
    private          Texture? texture;

    public SoftwareFrameBuffer(int width, int height)
    {
      Check.That(width > 0, "width > 0");
      Check.That(height > 0, "height > 0");

      pixmap = new Pixmap(width, height);
      Colors = pixmap.ToRegion();
    }

    public PixmapRegion Colors { get; }

    public int Width  => Colors.Width;
    public int Height => Colors.Height;

    public void Draw(SpriteBatch batch)
    {
      var device = batch.Device;

      texture ??= device.Factory.CreateTexture(Colors.Pixmap, filterMode: TextureFilterMode.Point);
      texture.Upload(Colors);

      batch.Draw(
        texture: texture,
        x: -device.Viewport.Width / 2f,
        y: -device.Viewport.Height / 2f,
        rotation: 0,
        width: device.Viewport.Width,
        height: device.Viewport.Height
      );
    }

    public void Clear(Color color)
    {
      Colors.Fill(color);
    }

    public void Dispose()
    {
      texture?.Dispose();
      pixmap.Dispose();
    }
  }
}