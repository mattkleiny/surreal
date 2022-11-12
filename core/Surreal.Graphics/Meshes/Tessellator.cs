using System.Runtime.InteropServices;
using Surreal.Collections;
using Surreal.Mathematics;

namespace Surreal.Graphics.Meshes;

/// <summary>A utility for tessellating shapes into meshes of vertex geometry.</summary>
public abstract class Tessellator
{
  /// <summary>The types of <see cref="Figure" /> supported.</summary>
  protected enum FigureType : byte
  {
    Line,
    Triangle,
    Circle,
    Rectangle
  }

  /// <summary>A type of object to be rendered; a <see cref="Figure" /> with a clipping rect.</summary>
  protected record struct ClippedFigure
  {
    public Rectangle ClippingRect;
    public Figure Figure;
  }

  /// <summary>A type of object to be rendered; a union of all possible types and a tag.</summary>
  [StructLayout(LayoutKind.Explicit)]
  protected record struct Figure
  {
    [FieldOffset(1)] public Circle Circle;
    [FieldOffset(1)] public Line Line;
    [FieldOffset(1)] public Rectangle Rectangle;
    [FieldOffset(0)] public FigureType Tag;
    [FieldOffset(1)] public Triangle Triangle;
  }

  protected record struct Line(Vector2 From, Vector2 To);
  protected record struct Triangle(Vector2 A, Vector2 B, Vector2 C);
  protected record struct Circle(Vector2 Center, float Radius);
}

/// <summary>A utility for tessellating shapes into meshes of vertex geometry.</summary>
public sealed class Tessellator<TVertex> : Tessellator
  where TVertex : unmanaged
{
  // TODO: convert this into internal shape representations and then add a 'tessellation' step at the end

  private readonly List<ClippedFigure> _figures = new();
  private readonly List<uint> _indices = new();
  private readonly List<TVertex> _vertices = new();

  public int FigureCount => _figures.Count;
  public uint VertexCount => (uint) _vertices.Count;
  public uint IndexCount => (uint) _indices.Count;

  public ReadOnlySpan<TVertex> Vertices => _vertices.AsSpan();
  public ReadOnlySpan<uint> Indices => _indices.AsSpan();

  public void BeginClipRect()
  {
    throw new NotImplementedException();
  }

  public void EndClipRect()
  {
    throw new NotImplementedException();
  }

  public void AddLine(Vector2 a, Vector2 b)
  {
    _figures.Add(new ClippedFigure
    {
      ClippingRect = Rectangle.Empty,
      Figure = new Figure
      {
        Tag = FigureType.Line,
        Line = new Line(a, b)
      }
    });
  }

  public void AddVertex(TVertex vertex)
  {
    _vertices.Add(vertex);
  }

  public void AddIndex(uint index)
  {
    _indices.Add((ushort) index);
  }

  public void Clear()
  {
    _vertices.Clear();
    _indices.Clear();
  }

  public void WriteTo(Mesh<TVertex> mesh)
  {
    mesh.Vertices.Write(Vertices);
    mesh.Indices.Write(Indices);
  }
}

/// <summary>Common extensions for <see cref="Tessellator{TVertex}" />s.</summary>
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



