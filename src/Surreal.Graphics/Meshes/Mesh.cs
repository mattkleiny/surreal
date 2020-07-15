using System;
using System.Diagnostics;
using Surreal.Graphics.Materials;

namespace Surreal.Graphics.Meshes {
  [DebuggerDisplay("Mesh with {Vertices.Length} vertices and {Indices.Length} indices")]
  public sealed class Mesh : IDisposable {
    private readonly IGraphicsDevice device;

    public static Mesh Create<TVertex>(IGraphicsDevice device)
        where TVertex : unmanaged {
      return new Mesh(device, VertexAttributeSet.Create<TVertex>());
    }

    private Mesh(IGraphicsDevice device, VertexAttributeSet attributes) {
      this.device = device;

      Attributes = attributes;

      Vertices = device.CreateBuffer();
      Indices  = device.CreateBuffer();
    }

    public GraphicsBuffer Vertices { get; }
    public GraphicsBuffer Indices  { get; }

    public VertexAttributeSet Attributes { get; }

    public void DrawImmediate(ShaderProgram shader, PrimitiveType type = PrimitiveType.Triangles) {
      DrawImmediate(shader, Vertices.Length, Indices.Length, type);
    }

    public void DrawImmediate(ShaderProgram shader, int vertexCount, int indexCount, PrimitiveType type = PrimitiveType.Triangles) {
      device.DrawMeshImmediate(this, shader, vertexCount, indexCount, type);
    }

    public void Dispose() {
      Vertices.Dispose();
      Indices.Dispose();
    }
  }
}