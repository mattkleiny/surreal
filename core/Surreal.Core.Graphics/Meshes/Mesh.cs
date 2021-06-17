using System;
using System.Diagnostics;
using Surreal.Graphics.Materials;

namespace Surreal.Graphics.Meshes {
  // TODO: create a mesh builder/mesh templates for common shapes

  [DebuggerDisplay("Mesh with {Vertices.Length} vertices and {Indices.Length} indices")]
  public sealed class Mesh<TVertex> : IDisposable
      where TVertex : unmanaged {
    private readonly IGraphicsDevice device;

    public Mesh(IGraphicsDevice device) {
      this.device = device;

      Vertices = device.CreateBuffer<TVertex>();
      Indices  = device.CreateBuffer<ushort>();

      Attributes = VertexAttributeSet.Create<TVertex>();
    }

    public GraphicsBuffer<TVertex> Vertices   { get; }
    public GraphicsBuffer<ushort>  Indices    { get; }
    public VertexAttributeSet      Attributes { get; }

    public void DrawImmediate(ShaderProgram shader, PrimitiveType type = PrimitiveType.Triangles) {
      DrawImmediate(shader, Vertices.Length, Indices.Length, type);
    }

    public void DrawImmediate(ShaderProgram shader, int vertexCount, int indexCount, PrimitiveType type = PrimitiveType.Triangles) {
      device.DrawMesh(this, shader, vertexCount, indexCount, type);
    }

    public void Dispose() {
      Vertices.Dispose();
      Indices.Dispose();
    }
  }
}