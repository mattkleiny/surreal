using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.SPI;
using Surreal.Platform;

namespace Surreal.Graphics {
  public sealed class GraphicsDevice : IGraphicsDevice {
    public GraphicsDevice(IGraphicsBackend backend, IPlatformHost host) {
      Backend = backend;

      Backend.Pipeline.Rasterizer.Viewport = new Viewport(host.Width, host.Height);
    }

    public IGraphicsBackend Backend  { get; }
    public IPipelineState   Pipeline => Backend.Pipeline;

    public void BeginFrame() {
      Backend.BeginFrame();
    }

    public void Clear(Color color) {
      Backend.ClearColorBuffer(color);
      Backend.ClearDepthBuffer();
    }

    public void DrawMeshImmediate(Mesh mesh, ShaderProgram shader, int vertexCount, int indexCount, PrimitiveType type = PrimitiveType.Triangles) {
      if (vertexCount == 0) return; // empty mesh? don't render

      Pipeline.ActiveShader       = shader;
      Pipeline.ActiveVertexBuffer = mesh.Vertices;
      Pipeline.ActiveIndexBuffer  = mesh.Indices;

      shader.Bind(mesh.Attributes);

      if (indexCount > 0) {
        Backend.DrawMeshIndexed(indexCount, type);
      } else {
        Backend.DrawMesh(vertexCount, type);
      }
    }

    public void EndFrame() {
      Backend.EndFrame();
      Backend.Present();
    }
  }
}