using Surreal.Collections;

namespace Surreal.Graphics.Meshes;

/// <summary>
/// A utility for tessellating shapes into meshes of vertex geometry.
/// </summary>
public sealed class MeshTessellator<TVertex>
  where TVertex : unmanaged
{
  private readonly List<uint> _indices = [];
  private readonly List<TVertex> _vertices = [];

  public uint VertexCount => (uint)_vertices.Count;
  public uint IndexCount => (uint)_indices.Count;

  /// <summary>
  /// Adds a vertex to the tessellator.
  /// </summary>
  public void AddVertex(TVertex vertex)
  {
    _vertices.Add(vertex);
  }

  /// <summary>
  /// Adds an index to the tessellator.
  /// </summary>
  public void AddIndex(uint index)
  {
    _indices.Add((ushort)index);
  }

  /// <summary>
  /// Clears all vertices and indices from the tessellator.
  /// </summary>
  public void Clear()
  {
    _vertices.Clear();
    _indices.Clear();
  }

  /// <summary>
  /// Write the tessellator vertices and indices to a <see cref="Mesh{TVertex}" />.
  /// </summary>
  public void WriteTo(Mesh<TVertex> mesh)
  {
    mesh.Vertices.Write(_vertices.AsSpan());
    mesh.Indices.Write(_indices.AsSpan());
  }

  /// <summary>
  /// Creates a <see cref="Mesh{TVertex}"/> from the tessellator.
  /// </summary>
  public Mesh<TVertex> ToMesh(IGraphicsDevice device, BufferUsage usage = BufferUsage.Static)
  {
    var mesh = new Mesh<TVertex>(device, usage);

    WriteTo(mesh);

    return mesh;
  }
}

/// <summary>
/// Common extensions for <see cref="MeshTessellator{TVertex}" />s.
/// </summary>
public static class MeshTessellatorExtensions
{
  /// <summary>
  /// Adds a simple line.
  /// </summary>
  public static void AddLine<TVertex>(this MeshTessellator<TVertex> tessellator, TVertex a, TVertex b)
    where TVertex : unmanaged
  {
    var offset = tessellator.VertexCount;

    tessellator.AddVertex(a);
    tessellator.AddVertex(b);

    tessellator.AddIndex(offset + 0);
    tessellator.AddIndex(offset + 1);
    tessellator.AddIndex(offset + 1);
  }

  /// <summary>
  /// Adds a simple triangle.
  /// </summary>
  public static void AddTriangle<TVertex>(this MeshTessellator<TVertex> tessellator, TVertex a, TVertex b, TVertex c)
    where TVertex : unmanaged
  {
    var offset = tessellator.VertexCount;

    tessellator.AddVertex(a);
    tessellator.AddVertex(b);
    tessellator.AddVertex(c);

    tessellator.AddIndex(offset + 0);
    tessellator.AddIndex(offset + 1);
    tessellator.AddIndex(offset + 2);
  }

  /// <summary>
  /// Adds a fan of triangles, starting at the first vertex.
  /// </summary>
  public static void AddTriangleFan<TVertex>(this MeshTessellator<TVertex> tessellator, ReadOnlySpan<TVertex> vertices)
    where TVertex : unmanaged
  {
    var first = tessellator.VertexCount;

    tessellator.AddVertex(vertices[0]);

    for (var i = 1; i < vertices.Length - 1; i++)
    {
      var offset = tessellator.VertexCount;

      tessellator.AddVertex(vertices[i + 0]);
      tessellator.AddVertex(vertices[i + 1]);

      tessellator.AddIndex(first);
      tessellator.AddIndex(offset + 0);
      tessellator.AddIndex(offset + 1);
    }
  }

  /// <summary>
  /// Adds a quad formed from 2 triangles.
  /// </summary>
  public static void AddQuad<TVertex>(this MeshTessellator<TVertex> tessellator, TVertex a, TVertex b, TVertex c, TVertex d)
    where TVertex : unmanaged
  {
    var offset = tessellator.VertexCount;

    tessellator.AddVertex(a);
    tessellator.AddVertex(b);
    tessellator.AddVertex(c);
    tessellator.AddVertex(d);

    tessellator.AddIndex(offset + 0);
    tessellator.AddIndex(offset + 1);
    tessellator.AddIndex(offset + 2);

    tessellator.AddIndex(offset + 0);
    tessellator.AddIndex(offset + 2);
    tessellator.AddIndex(offset + 3);
  }
}
