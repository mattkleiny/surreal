using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Experimental.Rendering {
  public readonly struct FrameBufferDescriptor {
    public int Width  { get; }
    public int Height { get; }
    public int Depth  { get; }

    public TextureFormat     Format     { get; }
    public TextureFilterMode FilterMode { get; }

    public FrameBufferDescriptor(int width, int height, int depth, TextureFormat format, TextureFilterMode filterMode) {
      Width      = width;
      Height     = height;
      Depth      = depth;
      Format     = format;
      FilterMode = filterMode;
    }
  }
}