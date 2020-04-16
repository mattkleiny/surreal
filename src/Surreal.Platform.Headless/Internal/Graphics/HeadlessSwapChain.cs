using Surreal.Graphics;
using Surreal.Graphics.SPI;

namespace Surreal.Platform.Internal.Graphics
{
  internal sealed class HeadlessSwapChain : ISwapChain
  {
    public void ClearColorBuffer(Color color)
    {
      // no-op
    }

    public void ClearDepthBuffer()
    {
      // no-op
    }

    public void Present()
    {
      // no-op
    }
  }
}
