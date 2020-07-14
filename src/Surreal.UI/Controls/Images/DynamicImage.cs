using Surreal.Graphics;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics.Timing;

namespace Surreal.UI.Controls.Images {
  public abstract class DynamicImage : Image {
    public ImageRegion Image   { get; }
    public bool        IsDirty { get; set; } = false;

    public DynamicImage(IGraphicsDevice device, Graphics.Textures.Image image)
      : base(device.CreateTexture(image).ToRegion()) {
      Image = image.ToRegion();
    }

    public override void Draw(DeltaTime deltaTime, SpriteBatch batch) {
      if (IsDirty) {
        Invalidate();
      }

      base.Draw(deltaTime, batch);
    }

    public void Invalidate() {
      Repaint(Image);
      Sprite.Texture.Upload(Image);

      IsDirty = false;
    }

    public override void Dispose() {
      base.Dispose();

      Image.Dispose();
    }

    protected abstract void Repaint(ImageRegion image);
  }
}