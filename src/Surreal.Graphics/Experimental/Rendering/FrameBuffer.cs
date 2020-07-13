using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Experimental.Rendering {
  public abstract class FrameBuffer : GraphicsResource {
    public abstract Texture Texture { get; }
  }
}