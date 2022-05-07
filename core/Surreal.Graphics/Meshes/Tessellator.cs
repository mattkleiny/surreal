﻿using Surreal.Collections;

namespace Surreal.Graphics.Meshes;

/// <summary>A utility for tessellating shapes into meshes of vertex geometry.</summary>
public sealed class Tessellator<TVertex>
  where TVertex : unmanaged
{
  private readonly List<TVertex> vertices = new();
  private readonly List<ushort> indices = new();

  public ushort VertexCount => (ushort) vertices.Count;
  public int    IndexCount  => indices.Count;

  public ReadOnlySpan<TVertex> Vertices => vertices.AsSpan();
  public ReadOnlySpan<ushort>  Indices  => indices.AsSpan();

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
    mesh.Vertices.Write(vertices.AsSpan());
    mesh.Indices.Write(indices.AsSpan());
  }
}

/// <summary>Common extensions for <see cref="Tessellator{TVertex}"/>s.</summary>
public static class TessellatorExtensions
{
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
    for (var i = 0; i < vertices.Length - 2; i++)
    {
      var offset = tessellator.VertexCount;

      tessellator.AddVertex(vertices[0]);
      tessellator.AddVertex(vertices[i + 1]);
      tessellator.AddVertex(vertices[i + 2]);

      tessellator.AddIndex(offset + 0);
      tessellator.AddIndex(offset + 1);
      tessellator.AddIndex(offset + 2);
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

  /// <summary>Adds a fan of triangles based on the centroid of the given set of vertices.</summary>
  public static void AddCentroidFan(this Tessellator<Vertex2> tessellator, ReadOnlySpan<Vertex2> vertices)
  {
    static Vector2 CalculateCentroid(ReadOnlySpan<Vertex2> vertices)
    {
      var center = Vector2.Zero;

      foreach (var vertex in vertices)
      {
        center += vertex.Position;
      }

      return center / vertices.Length;
    }

    // add center vertex
    var start = tessellator.VertexCount;

    tessellator.AddVertex(new Vertex2(CalculateCentroid(vertices), vertices[0].Color, new Vector2(0.5f, 0.5f)));

    // add remaining vertices, connected to center
    for (var i = 0; i < vertices.Length - 2; i++)
    {
      var offset = tessellator.VertexCount;

      tessellator.AddVertex(vertices[i]);
      tessellator.AddVertex(vertices[i + 1]);

      tessellator.AddIndex(start);
      tessellator.AddIndex(offset + 0);
      tessellator.AddIndex(offset + 1);
    }

    // connect the last triangle to the start and end of the fan
    tessellator.AddIndex(start);
    tessellator.AddIndex(start + 1);
    tessellator.AddIndex(tessellator.VertexCount);
  }
}