using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Surreal.Graphics;

internal sealed class SilkGraphicsBackend(IWindow window) : IGraphicsBackend
{
  public IGraphicsDevice CreateDevice(GraphicsMode mode)
  {
    return mode switch
    {
      GraphicsMode.Universal => new SilkGraphicsDeviceOpenGL(window.CreateOpenGL()),
      GraphicsMode.HighDefinition => new SilkGraphicsDeviceVulkan(),

      _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
    };
  }
}
