using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.SPI;

namespace Surreal.Graphics {
  public interface IGraphicsDevice {
    IGraphicsBackend Backend  { get; }
    IPipelineState   Pipeline { get; }

    Viewport Viewport {
      get => Pipeline.Rasterizer.Viewport;
      set => Pipeline.Rasterizer.Viewport = value;
    }

    void BeginFrame();
    void Clear(Color color);

    void DrawMeshImmediate(
        Mesh mesh,
        ShaderProgram shader,
        int vertexCount,
        int indexCount,
        PrimitiveType type = PrimitiveType.Triangles
    );

    void EndFrame();
  }
}