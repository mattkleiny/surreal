using System;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Experimental.Rendering;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.SPI;
using PrimitiveType = Surreal.Graphics.Meshes.PrimitiveType;

namespace Surreal.Platform.Internal.Graphics.Resources {
  internal sealed class OpenTKCommandBuffer : CommandBuffer {
    private readonly OpenTKGraphicsBackend backend;

    public OpenTKCommandBuffer(OpenTKGraphicsBackend backend) {
      this.backend = backend;
    }

    public IPipelineState Pipeline => backend.Pipeline;

    public override void SetRenderTarget(FrameBuffer target) {
      throw new NotImplementedException();
    }

    public override void ClearRenderTarget(Color color, bool clearColor, bool clearDepth) {
      if (clearColor) {
        GL.ClearColor(
            color.R / 255.0f,
            color.G / 255.0f,
            color.B / 255.0f,
            color.A / 255.0f
        );

        GL.Clear(ClearBufferMask.ColorBufferBit);
      }

      if (clearDepth) {
        GL.Clear(ClearBufferMask.DepthBufferBit);
      }
    }

    public override void DrawMesh(Mesh mesh, FrameBuffer source, FrameBuffer target, ShaderProgram shader) {
      Pipeline.ActiveShader       = shader;
      Pipeline.ActiveVertexBuffer = mesh.Vertices;
      Pipeline.ActiveIndexBuffer  = mesh.Indices;

      shader.Bind(mesh.Attributes);

      backend.DrawMeshIndexed(mesh.Indices.Count, PrimitiveType.Triangles);
    }

    public override void Flush() {
      GL.Flush();
    }
  }
}