using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.SPI;

namespace Surreal.Platform.Internal.Graphics {
  internal sealed class OpenTKSwapChain : ISwapChain {
    private readonly OpenTKWindow window;

    public OpenTKSwapChain(OpenTKWindow window) {
      this.window = window;
    }

    public void ClearColorBuffer(Color color) {
      GL.ClearColor(
          color.R / 255.0f,
          color.G / 255.0f,
          color.B / 255.0f,
          color.A / 255.0f
      );

      GL.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void ClearDepthBuffer() {
      GL.Clear(ClearBufferMask.DepthBufferBit);
    }

    public void Present() {
      window.Present();
    }
  }
}