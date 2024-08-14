using Silk.NET.OpenGL;

namespace Surreal.Graphics;

internal sealed class SilkGraphicsBackend(GL gl) : IGraphicsBackend
{
  public IGraphicsDevice CreateDevice()
  {
    return new SilkGraphicsDevice(gl);
  }
}
