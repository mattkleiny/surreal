using Surreal.Collections;

namespace Surreal.Graphics.Meshes;

/// <summary>A utility for tessellating shapes into <see cref="Mesh{TVertex}"/>es.</summary>
public sealed class Tessellator<TVertex>
  where TVertex : unmanaged
{
  private readonly List<TVertex> vertices = new();
  private readonly List<ushort> indices = new();

  public ushort VertexCount => (ushort) vertices.Count;
  public int    IndexCount  => indices.Count;

  public void AddVertex(TVertex vertex)
  {
    vertices.Add(vertex);
  }

  public void AddIndex(int index)
  {
    indices.Add((ushort) index);
  }

  public void Clear()
  {
    vertices.Clear();
    indices.Clear();
  }

  public void WriteVerticesTo(Span<TVertex> span)
  {
    vertices.AsSpan().CopyTo(span);
  }

  public void WriteIndicesTo(Span<ushort> span)
  {
    indices.AsSpan().CopyTo(span);
  }

  public void WriteTo(Mesh<TVertex> mesh)
  {
    mesh.Vertices.Write(vertices.AsSpan());
    mesh.Indices.Write(indices.AsSpan());
  }
}

/// <summary>Common extensions for <see cref="Tessellator{TVertex}"/>s.</summary>
public static class TessellatorExtensions
{
  public static void AddTriangle<TVertex>(this Tessellator<TVertex> tessellator, TVertex a, TVertex b, TVertex c)
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

  public static void AddQuad<TVertex>(this Tessellator<TVertex> tessellator, TVertex a, TVertex b, TVertex c, TVertex d)
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
