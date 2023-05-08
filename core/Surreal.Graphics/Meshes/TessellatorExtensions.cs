namespace Surreal.Graphics.Meshes;

/// <summary>
/// Common extensions for <see cref="Tessellator{TVertex}" />s.
/// </summary>
public static class TessellatorExtensions
{
  /// <summary>
  /// Adds a simple line.
  /// </summary>
  public static void AddLine<TVertex>(this Tessellator<TVertex> tessellator, TVertex a, TVertex b)
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

  /// <summary>
  /// Adds a fan of triangles, starting at the first vertex.
  /// </summary>
  public static void AddTriangleFan<TVertex>(this Tessellator<TVertex> tessellator, ReadOnlySpan<TVertex> vertices)
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
