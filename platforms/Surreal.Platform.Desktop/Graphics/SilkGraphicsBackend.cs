using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Surreal.Graphics;

internal sealed class SilkGraphicsBackend(DesktopConfiguration configuration, IWindow window) : IGraphicsBackend
{
  public IGraphicsDevice CreateDevice(GraphicsMode mode)
  {
    return configuration.GraphicsProvider switch
    {
      GraphicsProvider.OpenGL => new SilkGraphicsDeviceOpenGL(window.CreateOpenGL()),
      GraphicsProvider.WebGPU => new SilkGraphicsDeviceWebGpu(window, mode),

      _ => throw new InvalidOperationException("An unsupported GraphicsProvider was provided")
    };
  }
}
