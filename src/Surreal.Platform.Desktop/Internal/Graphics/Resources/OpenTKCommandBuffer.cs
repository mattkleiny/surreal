using System;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Experimental.Rendering;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;

namespace Surreal.Platform.Internal.Graphics.Resources {
  internal sealed class OpenTKCommandBuffer : CommandBuffer {
    private readonly OpenTKGraphicsDevice device;

    public OpenTKCommandBuffer(OpenTKGraphicsDevice device) {
      this.device = device;
    }

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
      device.DrawMeshImmediate(mesh, shader, mesh.Vertices.Length, mesh.Indices.Length);
    }

    public override void Flush() {
      GL.Flush();
    }
  }
}