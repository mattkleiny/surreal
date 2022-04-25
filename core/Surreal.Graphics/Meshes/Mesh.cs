using System.Runtime.InteropServices;
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
}

/// <summary>Abstracts over all possible <see cref="Mesh{TVertex}"/> types.</summary>
public abstract class Mesh : IDisposable
{
  /// <summary>Builds a full-screen <see cref="Mesh"/> quad.</summary>
  public static Mesh BuildFullscreenQuad(IGraphicsServer server)
  {
    var mesh = new Mesh<Vertex>(server);

    mesh.Vertices.Write(stackalloc[]
    {
      new Vertex(new(-1f, -1f), new(0f, 1f)),
      new Vertex(new(-1f, 1f), new(0f, 0f)),
      new Vertex(new(1f, 1f), new(1f, 0f)),
      new Vertex(new(1f, -1f), new(1f, 1f)),
    });

    mesh.Indices.Write(stackalloc ushort[]
    {
      0, 1, 2,
      0, 2, 3,
    });

    return mesh;
  }

  /// <summary>The <see cref="VertexDescriptorSet"/> for the mesh.</summary>
  public abstract VertexDescriptorSet Descriptors { get; }

  /// <summary>Draws the mesh with the given <see cref="ShaderProgram"/>.</summary>
  public abstract void Draw(ShaderProgram shader, MeshType type = MeshType.Triangles);

  /// <summary>Draws the mesh with the given <see cref="ShaderProgram"/> and primitive counts.</summary>
  public abstract void Draw(ShaderProgram shader, int vertexCount, int indexCount, MeshType type = MeshType.Triangles);

  /// <summary>Disposes of the mesh, freeing any of it's allocated resources.</summary>
  public abstract void Dispose();

  /// <summary>A simple vertex type for primitive shapes.</summary>
  [StructLayout(LayoutKind.Sequential)]
  private record struct Vertex(Vector2 Position, Vector2 UV)
  {
    [VertexDescriptor(
      Count = 2,
      Type = VertexType.Float
    )]
    public Vector2 Position = Position;

    [VertexDescriptor(
      Count = 2,
      Type = VertexType.Float
    )]
    public Vector2 UV = UV;
  }
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
