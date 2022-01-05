using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Images;
using Surreal.Graphics.Textures;

namespace Surreal.Internal.Graphics.Resources;

[DebuggerDisplay("Render Target {Texture.Width}x{Texture.Height} @ {Texture.Format} ~{Texture.Size}")]
internal sealed class OpenTkRenderTexture : RenderTexture, IHasNativeId
{
  public int Id { get; } = GL.GenFramebuffer();

  public override Texture Texture { get; }
  public          Image   Image   { get; }

  public OpenTkRenderTexture(Texture texture, Image image)
  {
    Texture = texture;
    Image   = image;
  }

  protected override void Dispose(bool managed)
  {
    GL.DeleteFramebuffer(Id);

    if (managed)
    {
      Texture.Dispose();
      Image.Dispose();
    }

    base.Dispose(managed);
  }
}
