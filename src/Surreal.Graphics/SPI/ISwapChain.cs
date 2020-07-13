namespace Surreal.Graphics.SPI {
  public interface ISwapChain {
    void ClearColorBuffer(Color color);
    void ClearDepthBuffer();
    void Present();
  }
}