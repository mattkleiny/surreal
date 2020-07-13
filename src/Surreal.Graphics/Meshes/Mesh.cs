using System;
using System.Diagnostics;
using Surreal.Graphics.Materials;
using Surreal.IO;

namespace Surreal.Graphics.Meshes {
  [DebuggerDisplay("Mesh with {Vertices.Count} vertices and {Indices.Count} indices ~{Size}")]
  public sealed class Mesh : IDisposable {
    private readonly IGraphicsDevice device;

    public static Mesh Create<TVertex>(IGraphicsDevice device)
        where TVertex : unmanaged {
      return new Mesh(device, VertexAttributeSet.Create<TVertex>());
    }

    private Mesh(IGraphicsDevice device, VertexAttributeSet attributes) {
      this.device = device;

      Attributes = attributes;

      Vertices = device.Backend.CreateBuffer(stride: attributes.Stride);
      Indices  = device.Backend.CreateBuffer(stride: sizeof(ushort));
    }

    public GraphicsBuffer Vertices { get; }
    public GraphicsBuffer Indices  { get; }

    public VertexAttributeSet Attributes { get; }

    public int  TriangleCount => Vertices.Count / 3;
    public Size Size          => Vertices.Size + Indices.Size;

    public void DrawImmediate(ShaderProgram shader, PrimitiveType type = PrimitiveType.Triangles) {
      DrawImmediate(shader, Vertices.Count, Indices.Count, type);
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