using Surreal.Graphics;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Timing;

namespace Surreal.UI.Controls.Images
{
  public abstract class RawImage : Image
  {
    public Pixmap Pixmap  { get; }
    public bool   IsDirty { get; set; } = false;

    public RawImage(IGraphicsDevice device, Pixmap pixmap)
      : base(device.Factory.CreateTexture(pixmap))
    {
      Pixmap = pixmap;
    }

    public override void Draw(DeltaTime deltaTime, SpriteBatch batch)
    {
      if (IsDirty)
      {
        Invalidate();
      }

      base.Draw(deltaTime, batch);
    }

    public void Invalidate()
    {
      Repaint(Pixmap);
      Sprite.Texture.Upload(Pixmap);

      IsDirty = false;
    }

    public override void Dispose()
    {
      base.Dispose();

      Pixmap.Dispose();
    }

    protected abstract void Repaint(PixmapRegion pixmap);
  }
}