using System;
using System.Diagnostics;
using Surreal.Graphics.Materials;

namespace Surreal.Graphics.Meshes {
  public enum PrimitiveType {
    Points,
    Lines,
    LineStrip,
    LineLoop,
    Triangles,
    Quads,
    QuadStrip,
  }

  [DebuggerDisplay("Mesh with {Vertices.Length} vertices and {Indices.Length} indices")]
  public sealed class Mesh<TVertex> : IDisposable
      where TVertex : unmanaged {
    private readonly IGraphicsDevice device;

    public Mesh(IGraphicsDevice device) {
      this.device = device;

      Vertices    = device.CreateBuffer<TVertex>();
      Indices     = device.CreateBuffer<ushort>();
      Descriptors = VertexDescriptorSet.Create<TVertex>();
    }

    public GraphicsBuffer<TVertex> Vertices    { get; }
    public GraphicsBuffer<ushort>  Indices     { get; }
    public VertexDescriptorSet     Descriptors { get; }

    public void DrawImmediate(Material.Pass pass, PrimitiveType type = PrimitiveType.Triangles) {
      DrawImmediate(pass, Vertices.Length, Indices.Length, type);
    }

    public void DrawImmediate(Material.Pass pass, int vertexCount, int indexCount, PrimitiveType type = PrimitiveType.Triangles) {
      device.DrawMesh(this, pass, vertexCount, indexCount, type);
    }

    public void Dispose() {
      Vertices.Dispose();
      Indices.Dispose();
    }
  }
}