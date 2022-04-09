using Surreal.Graphics.Shaders;

namespace Surreal.Graphics.Meshes;

/// <summary>Different types of supported primitive shapes for meshes.</summary>
public enum MeshType
{
  Points,
  Lines,
  LineStrip,
  LineLoop,
  Triangles,
  Quads,
  QuadStrip,
}

/// <summary>Abstracts over all possible <see cref="Mesh{TVertex}"/> types.</summary>
public abstract class Mesh : IDisposable
{
  /// <summary>The <see cref="VertexDescriptorSet"/> for the mesh.</summary>
  public abstract VertexDescriptorSet Descriptors { get; }

  /// <summary>Disposes of the mesh, freeing any of it's allocated resources.</summary>
  public abstract void Dispose();
}

/// <summary>A mesh with a strongly-typed vertex type, <see cref="TVertex"/>.</summary>
[DebuggerDisplay("Mesh with {Vertices.Length} vertices and {Indices.Length} indices")]
public sealed class Mesh<TVertex> : Mesh
  where TVertex : unmanaged
{
  private static VertexDescriptorSet VertexDescriptorSet { get; } = VertexDescriptorSet.Create<TVertex>();

  private readonly IGraphicsServer server;

  public Mesh(IGraphicsServer server)
  {
    this.server = server;

    Vertices = new GraphicsBuffer<TVertex>(server);
    Indices = new GraphicsBuffer<ushort>(server);
  }

  public GraphicsBuffer<TVertex> Vertices { get; }
  public GraphicsBuffer<ushort>  Indices  { get; }

  public override VertexDescriptorSet Descriptors => VertexDescriptorSet;

  public void DrawImmediate(Material material, MeshType type = MeshType.Triangles)
  {
    DrawImmediate(material, Vertices.Length, Indices.Length, type);
  }

  public void DrawImmediate(Material material, int vertexCount, int indexCount, MeshType type = MeshType.Triangles)
  {
    throw new NotImplementedException();
  }

  public override void Dispose()
  {
    Vertices.Dispose();
    Indices.Dispose();
  }
}
