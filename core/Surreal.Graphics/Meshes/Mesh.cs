using Surreal.Graphics.Materials;

namespace Surreal.Graphics.Meshes;

/// <summary>Abstracts over all possible <see cref="Mesh{TVertex}"/> types.</summary>
public abstract class Mesh : IDisposable
{
  public abstract void Dispose();
}

/// <summary>A mesh with a strongly-typed vertex type, <see cref="TVertex"/>.</summary>
[DebuggerDisplay("Mesh with {Vertices.Length} vertices and {Indices.Length} indices")]
public sealed class Mesh<TVertex> : Mesh
  where TVertex : unmanaged
{
  private static VertexDescriptorSet VertexDescriptorSet { get; } = VertexDescriptorSet.Create<TVertex>();

  private readonly IGraphicsDevice device;

  public Mesh(IGraphicsDevice device)
  {
    this.device = device;

    Vertices = device.CreateBuffer<TVertex>();
    Indices  = device.CreateBuffer<ushort>();
  }

  public GraphicsBuffer<TVertex> Vertices    { get; }
  public GraphicsBuffer<ushort>  Indices     { get; }
  public VertexDescriptorSet     Descriptors => VertexDescriptorSet;

  public void DrawImmediate(
    Material material,
    MeshType type = MeshType.Triangles
  )
  {
    DrawImmediate(material, Vertices.Length, Indices.Length, type);
  }

  public void DrawImmediate(
    Material material,
    int vertexCount,
    int indexCount,
    MeshType type = MeshType.Triangles
  )
  {
    device.DrawMesh(this, material, vertexCount, indexCount, type);
  }

  public override void Dispose()
  {
    Vertices.Dispose();
    Indices.Dispose();
  }
}
