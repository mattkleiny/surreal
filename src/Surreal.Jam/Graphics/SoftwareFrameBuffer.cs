using System;
using System.Diagnostics;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics {
  public sealed class SoftwareFrameBuffer : IDisposable {
    private readonly Image   image;
    private          Texture? texture;

    public SoftwareFrameBuffer(int width, int height) {
      Debug.Assert(width  > 0, "width > 0");
      Debug.Assert(height > 0, "height > 0");

      image = new Image(width, height);
      Colors = image.ToRegion();
    }

    public ImageRegion Colors { get; }

    public int Width  => Colors.Width;
    public int Height => Colors.Height;

    public void Draw(SpriteBatch batch) {
      var device = batch.Device;

      texture ??= device.Backend.CreateTexture(Colors.Image, filterMode: TextureFilterMode.Point);
      texture.Upload(Colors);

      batch.Draw(
          texture: texture,
          x: -device.Viewport.Width  / 2f,
          y: -device.Viewport.Height / 2f,
          rotation: Angle.Zero, 
          width: device.Viewport.Width,
          height: device.Viewport.Height
      );
    }

    public void Clear(Color color) {
      Colors.Fill(color);
    }

    public void Dispose() {
      texture?.Dispose();
      image.Dispose();
    }
  }
}