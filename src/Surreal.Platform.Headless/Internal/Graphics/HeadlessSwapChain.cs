using Surreal.Graphics;

namespace Surreal.Platform.Internal.Graphics {
  internal sealed class HeadlessSwapChain {
    public void ClearColorBuffer(Color color) {
      // no-op
    }

    public void ClearDepthBuffer() {
      // no-op
    }

    public void Present() {
      // no-op
    }
  }
}