using System.Diagnostics;
using Surreal.Graphics.Materials;

namespace Surreal.Graphics.Meshes;

/// <summary>A mesh with a strongly-typed vertex type, <see cref="TVertex"/>.</summary>
[DebuggerDisplay("Mesh with {Vertices.Length} vertices and {Indices.Length} indices")]
public sealed class Mesh<TVertex> : IDisposable
  where TVertex : unmanaged
{
  private static VertexDescriptorSet SharedDescriptors { get; } = VertexDescriptorSet.Create<TVertex>();

  private readonly IGraphicsDevice device;

  public Mesh(IGraphicsDevice device)
  {
    this.device = device;

    Vertices = device.CreateBuffer<TVertex>();
    Indices  = device.CreateBuffer<ushort>();
  }

  public GraphicsBuffer<TVertex> Vertices    { get; }
  public GraphicsBuffer<ushort>  Indices     { get; }
  public VertexDescriptorSet     Descriptors => SharedDescriptors;

  public void DrawImmediate(Material material, PrimitiveType type = PrimitiveType.Triangles)
  {
    DrawImmediate(material, Vertices.Length, Indices.Length, type);
  }

  public void DrawImmediate(Material material, int vertexCount, int indexCount, PrimitiveType type = PrimitiveType.Triangles)
  {
    device.DrawMesh(this, material, vertexCount, indexCount, type);
  }

  public void Dispose()
  {
    Vertices.Dispose();
    Indices.Dispose();
  }
}
