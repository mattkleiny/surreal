using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Rendering {
  public abstract class FrameBuffer : GraphicsResource {
    public abstract Texture Texture { get; }
  }
}