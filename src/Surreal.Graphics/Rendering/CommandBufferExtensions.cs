using System.Numerics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;

namespace Surreal.Graphics.Rendering {
  public static class CommandBufferExtensions {
    private static ShaderProgram? BlitProgram;
    private static Mesh?          FullscreenMesh;

    public static void Blit(this CommandBuffer buffer, IGraphicsDevice device, FrameBuffer source, FrameBuffer target) {
      BlitProgram ??= CreateBlitProgram(device);

      Blit(buffer, device, source, target, BlitProgram);
    }

    public static void Blit(this CommandBuffer buffer, IGraphicsDevice device, FrameBuffer source, FrameBuffer target, ShaderProgram program) {
      FullscreenMesh ??= CreateFullscreenMesh(device);

      buffer.DrawMesh(FullscreenMesh, source, target, program);
    }

    private static ShaderProgram CreateBlitProgram(IGraphicsDevice device) {
      return device.Factory.CreateShaderProgram(
          Shader.LoadAsync(ShaderType.Vertex, "resx://Surreal.Graphics/Resources/Shaders/Blit.vert.glsl").Result,
          Shader.LoadAsync(ShaderType.Fragment, "resx://Surreal.Graphics/Resources/Shaders/Blit.frag.glsl").Result
      );
    }

    private static Mesh CreateFullscreenMesh(IGraphicsDevice device) {
      var mesh = new Mesh(device, VertexAttributes.FromVertex<Vertex>());

      mesh.Vertices.Put(stackalloc[] {
          new Vertex(0, 0),
          new Vertex(0, 1),
          new Vertex(1, 1),
          new Vertex(1, 0)
      });

      mesh.Indices.Put(stackalloc[] {
          0, 1, 2,
          2, 3, 1
      });

      return mesh;
    }

    private struct Vertex {
      [VertexAttribute(
          Alias      = "a_Position",
          Count      = 2,
          Type       = VertexType.Float,
          Normalized = true
      )]
      public Vector2 Position;

      public Vertex(float x, float y)
          : this(new Vector2(x, y)) {
      }

      public Vertex(Vector2 position) {
        Position = position;
      }
    }
  }
}