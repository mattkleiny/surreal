using Surreal.Graphics.Meshes;

namespace Surreal.Graphics.SPI {
  public interface IGraphicsBackend {
    IGraphicsFactory Factory   { get; }
    ISwapChain       SwapChain { get; }
    IPipelineState   Pipeline  { get; }

    void BeginFrame();
    void DrawMeshIndexed(int count, PrimitiveType type);
    void DrawMesh(int count, PrimitiveType type);
    void EndFrame();
  }
}