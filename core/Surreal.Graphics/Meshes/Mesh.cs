﻿using Surreal.Graphics.Shaders;

namespace Surreal.Graphics.Meshes;

/// <summary>Different types of supported primitive shapes for meshes.</summary>
public enum MeshType
{
  Points,
  Lines,
  LineStrip,
  LineLoop,
  Triangles,
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
  /// <summary>Descriptors are statically shared amongst all meshes.</summary>
  private static VertexDescriptorSet SharedDescriptors { get; } = VertexDescriptorSet.Create<TVertex>();

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

  public override VertexDescriptorSet Descriptors => SharedDescriptors;

  public void Draw(ShaderProgram shader, MeshType type = MeshType.Triangles)
  {
    Draw(shader, Vertices.Length, Indices.Length, type);
  }

  public void Draw(ShaderProgram shader, int vertexCount, int indexCount, MeshType type = MeshType.Triangles)
  {
    server.DrawMesh(
      mesh: handle,
      shader: shader.Handle,
      vertices: Vertices.Handle,
      indices: Indices.Handle,
      descriptors: Descriptors,
      vertexCount: vertexCount,
      indexCount: indexCount,
      meshType: type,
      indexType: Indices.Type
    );
  }

  public override void Dispose()
  {
    Vertices.Dispose();
    Indices.Dispose();

    server.DeleteMesh(handle);
  }
}
