﻿using System.Runtime.CompilerServices;
using Surreal.Graphics.Shaders;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

/// <summary>Different types of supported primitive shapes for meshes.</summary>
public enum MeshType
{
  Points,
  Lines,
  LineStrip,
  LineLoop,
  Triangles
}

/// <summary>Abstracts over all possible <see cref="Mesh{TVertex}"/> types.</summary>
public abstract class Mesh : GraphicsResource, IHasSizeEstimate
{
  /// <summary>Builds a full-screen <see cref="Mesh"/> quad.</summary>
  public static Mesh CreateQuad(IGraphicsServer server, float size = 1f)
  {
    var mesh = new Mesh<Vertex2>(server);

    mesh.Vertices.Write(stackalloc[]
    {
      new Vertex2(new(-size, -size), Color.White, new(0f, 1f)),
      new Vertex2(new(-size, size), Color.White, new(0f, 0f)),
      new Vertex2(new(size, size), Color.White, new(1f, 0f)),
      new Vertex2(new(size, -size), Color.White, new(1f, 1f))
    });

    mesh.Indices.Write(stackalloc ushort[]
    {
      0, 1, 2,
      0, 2, 3
    });

    return mesh;
  }

  public abstract Size Size { get; }

  /// <summary>Draws the mesh with the given <see cref="ShaderProgram"/>.</summary>
  public abstract void Draw(ShaderProgram shader, MeshType type = MeshType.Triangles);

  /// <summary>Draws the mesh with the given <see cref="ShaderProgram"/> and primitive counts.</summary>
  public abstract void Draw(ShaderProgram shader, int vertexCount, int indexCount, MeshType type = MeshType.Triangles);
}

/// <summary>A mesh with a strongly-typed vertex type, <see cref="TVertex"/>.</summary>
[DebuggerDisplay("Mesh with {Vertices.Length} vertices and {Indices.Length} indices")]
public sealed class Mesh<TVertex> : Mesh
  where TVertex : unmanaged
{
  /// <summary>Descriptors are statically shared amongst all <see cref="TVertex"/>.</summary>
  private static readonly VertexDescriptorSet VertexDescriptors = VertexDescriptorSet.Create<TVertex>();

  private readonly IGraphicsServer server;
  private readonly GraphicsHandle handle;

  public Mesh(IGraphicsServer server)
  {
    this.server = server;

    Vertices = new GraphicsBuffer<TVertex>(server);
    Indices  = new GraphicsBuffer<ushort>(server);

    handle = server.CreateMesh();
  }

  public GraphicsBuffer<TVertex> Vertices { get; }
  public GraphicsBuffer<ushort>  Indices  { get; }

  public override Size Size => Vertices.Size + Indices.Size;

  public Tessellator<TVertex> CreateTessellator()
  {
    return new Tessellator<TVertex>();
  }

  public override void Draw(ShaderProgram shader, MeshType type = MeshType.Triangles)
  {
    Draw(shader, Vertices.Length, Indices.Length, type);
  }

  public override void Draw(ShaderProgram shader, int vertexCount, int indexCount, MeshType type = MeshType.Triangles)
  {
    server.DrawMesh(
      mesh: handle,
      shader: shader.Handle,
      vertices: Vertices.Handle,
      indices: Indices.Handle,
      descriptors: VertexDescriptors,
      vertexCount: vertexCount,
      indexCount: indexCount,
      meshType: type,
      indexType: Indices.Type
    );
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      Vertices.Dispose();
      Indices.Dispose();

      server.DeleteMesh(handle);
    }

    base.Dispose(managed);
  }
}
