using Surreal.Collections;

namespace Surreal.Graphics.Meshes;

/// <summary>A utility for tessellating shapes into meshes of vertex geometry.</summary>
public sealed class Tessellator<TVertex>
  where TVertex : unmanaged
{
  // TODO: convert this into internal shape representations and then add a 'tessellation' step at the end
  // TODO: consider using a stream packing method to make it efficient at runtime

  private readonly List<TVertex> vertices = new();
  private readonly List<uint> indices = new();

  public ushort VertexCount => (ushort) vertices.Count;
  public uint   IndexCount  => (uint) indices.Count;

  public ReadOnlySpan<TVertex> Vertices => vertices.AsSpan();
  public ReadOnlySpan<uint>    Indices  => indices.AsSpan();

  public void BeginClipRect()
  {
    throw new NotImplementedException();
  }

  public void EndClipRect()
  {
    throw new NotImplementedException();
  }

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

  public void WriteTo(Mesh<TVertex> mesh)
  {
    mesh.Vertices.Write(Vertices);
    mesh.Indices.Write(Indices);
  }
}

/// <summary>Common extensions for <see cref="Tessellator{TVertex}"/>s.</summary>
public static class TessellatorExtensions
{
  /// <summary>Adds a simple line.</summary>
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

  /// <summary>Adds a simple triangle.</summary>
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

  /// <summary>Adds a fan of triangles, starting at the first vertex.</summary>
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

  /// <summary>Adds a quad formed from 2 triangles.</summary>
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

/// <summary>A stream of geometry instructions packed into an efficient array to make it simpler to use.</summary>
public sealed class GeometryStream : IDisposable
{
  private readonly MemoryStream stream = Pool<MemoryStream>.Shared.CreateOrRent();
  private readonly BinaryWriter writer;

  // TODO: work on this

  public GeometryStream()
  {
    writer = new BinaryWriter(stream);
  }

  public void AddCircle(Vector2 center, float radius)
  {
    writer.Write((byte) ShapeKind.Circle);
    writer.Write(center.X);
    writer.Write(center.Y);
    writer.Write(radius);
  }

  public void AddTriangle(Vector2 a, Vector2 b, Vector2 c)
  {
    writer.Write((byte) ShapeKind.Triangle);
    writer.Write(a.X);
    writer.Write(a.Y);
    writer.Write(b.X);
    writer.Write(b.Y);
    writer.Write(c.X);
    writer.Write(c.Y);
  }

  public void AddQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
  {
    writer.Write((byte) ShapeKind.Quad);
    writer.Write(a.X);
    writer.Write(a.Y);
    writer.Write(b.X);
    writer.Write(b.Y);
    writer.Write(c.X);
    writer.Write(c.Y);
    writer.Write(d.X);
    writer.Write(d.Y);
  }

  public void AddPoints(ReadOnlySpan<Vector2> points)
  {
    writer.Write((byte) ShapeKind.Points);
    writer.Write(points.Length);

    foreach (var point in points)
    {
      writer.Write(point.X);
      writer.Write(point.Y);
    }
  }

  public void Dispose()
  {
    Pool<MemoryStream>.Shared.Return(stream);
  }

  private enum ShapeKind : byte
  {
    Circle,
    Triangle,
    Quad,
    Points,
  }
}
