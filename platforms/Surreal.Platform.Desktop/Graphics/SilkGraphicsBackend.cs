using Silk.NET.OpenGL;

namespace Surreal.Graphics;

internal sealed class SilkGraphicsBackend(GL gl) : IGraphicsBackend
{
  public IGraphicsDevice CreateDevice(GraphicsDeviceDescriptor descriptor)
  {
    return new SilkGraphicsDevice(gl);
  }
}
