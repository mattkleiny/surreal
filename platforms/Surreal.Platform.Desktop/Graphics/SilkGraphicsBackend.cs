using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Surreal.Graphics;

/// <summary>
/// A <see cref="IGraphicsBackend"/> implementation for Silk.NET.
/// </summary>
internal sealed class SilkGraphicsBackend(DesktopConfiguration configuration, IWindow window) : IGraphicsBackend
{
  public IGraphicsDevice CreateDevice(GraphicsMode mode)
  {
    return configuration.GraphicsProvider switch
    {
      GraphicsProvider.OpenGL => new SilkGraphicsDeviceOpenGL(window.CreateOpenGL()),
      GraphicsProvider.WGPU => new SilkGraphicsDeviceWGPU(window, mode),

      _ => throw new InvalidOperationException("An unsupported GraphicsProvider was provided")
    };
  }
}
