using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;

namespace Surreal.Platform.Internal.Graphics.Resources {
  [DebuggerDisplay("Render Target {Texture.Width}x{Texture.Height} @ {Texture.Format} ~{Texture.Size}")]
  internal sealed class OpenTKFrameBuffer : FrameBuffer {
    public readonly int Id = GL.GenFramebuffer();

    public override Texture Texture { get; }
    public          Image  Image  { get; }

    public OpenTKFrameBuffer(Texture texture, Image image) {
      Texture = texture;
      Image  = image;
    }

    protected override void Dispose(bool managed) {
      GL.DeleteFramebuffer(Id);

      if (managed) {
        Texture.Dispose();
        Image.Dispose();
      }

      base.Dispose(managed);
    }
  }
}