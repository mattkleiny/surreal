using Surreal.Graphics.Materials;
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
  /// <summary>Convenience method for building <see cref="Mesh{TVertex}"/>s with a <see cref="Tessellator{TVertex}"/>.</summary>
  public static Mesh<TVertex> Create<TVertex>(IGraphicsServer server, Action<Tessellator<TVertex>> builder)
    where TVertex : unmanaged
  {
    var mesh = new Mesh<TVertex>(server);

    mesh.Tessellate(builder);

    return mesh;
  }

  /// <summary>Builds a simple triangle <see cref="Mesh"/>.</summary>
  public static Mesh<Vertex2> CreateTriangle(IGraphicsServer server, float size = 1f)
  {
    return Create<Vertex2>(server, tessellator =>
    {
      tessellator.AddTriangle(
        new Vertex2(new(-size, -size), Color.White, new(0f, 0f)),
        new Vertex2(new(0f, size), Color.White, new(0.5f, 1f)),
        new Vertex2(new(size, -size), Color.White, new(1f, 0f))
      );
    });
  }

  /// <summary>Builds a simple quad <see cref="Mesh"/> quad.</summary>
  public static Mesh<Vertex2> CreateQuad(IGraphicsServer server, float size = 1f)
  {
    return Create<Vertex2>(server, tessellator =>
    {
      tessellator.AddQuad(
        new Vertex2(new(-size, -size), Color.White, new(0f, 1f)),
        new Vertex2(new(-size, size), Color.White, new(0f, 0f)),
        new Vertex2(new(size, size), Color.White, new(1f, 0f)),
        new Vertex2(new(size, -size), Color.White, new(1f, 1f))
      );
    });
  }

  /// <summary>Builds a simple circle <see cref="Mesh"/>.</summary>
  public static Mesh<Vertex2> CreateCircle(IGraphicsServer server, float radius = 1f, int segments = 16)
  {
    return Create<Vertex2>(server, tessellator =>
    {
      var vertices = new SpanList<Vertex2>(stackalloc Vertex2[segments]);
      var theta = 0f;

      for (var i = 0; i < segments; i++)
      {
        theta += 2 * MathF.PI / segments;

        var cos = MathF.Cos(theta);
        var sin = MathF.Sin(theta);

        var x = radius * cos;
        var y = radius * sin;

        var u = (cos + 1f) / 2f;
        var v = (sin + 1f) / 2f;

        vertices.Add(new Vertex2(new(x, y), Color.White, new(u, v)));
      }

      tessellator.AddTriangleFan(vertices);
    });
  }

  public abstract Size Size { get; }

  /// <summary>Draws the mesh with the given <see cref="ShaderProgram"/>.</summary>
  public abstract void Draw(ShaderProgram shader, MeshType type = MeshType.Triangles);

  /// <summary>Draws the mesh with the given <see cref="ShaderProgram"/> and primitive counts.</summary>
  public abstract void Draw(ShaderProgram shader, int vertexCount, int indexCount, MeshType type = MeshType.Triangles);

  /// <summary>Draws the mesh with the given <see cref="Material"/>.</summary>
  public abstract void Draw(Material material, MeshType type = MeshType.Triangles);

  /// <summary>Draws the mesh with the given <see cref="Material"/> and primitive counts.</summary>
  public abstract void Draw(Material material, int vertexCount, int indexCount, MeshType type = MeshType.Triangles);
}

/// <summary>A mesh with a strongly-typed vertex type, <see cref="TVertex"/>.</summary>
[DebuggerDisplay("Mesh with {Vertices.Length} vertices and {Indices.Length} indices")]
public sealed class Mesh<TVertex> : Mesh
  where TVertex : unmanaged
{
  /// <summary>Descriptors are statically shared amongst all <see cref="TVertex"/>.</summary>
  private static readonly VertexDescriptorSet VertexDescriptors = VertexDescriptorSet.Create<TVertex>();

  private readonly IGraphicsServer server;

  public Mesh(IGraphicsServer server, BufferUsage usage = BufferUsage.Static)
  {
    this.server = server;

    Vertices = new GraphicsBuffer<TVertex>(server, BufferType.Vertex, usage);
    Indices  = new GraphicsBuffer<uint>(server, BufferType.Index, usage);

    Handle = server.CreateMesh(Vertices.Handle, Indices.Handle, VertexDescriptors);
  }

  public GraphicsHandle          Handle   { get; }
  public GraphicsBuffer<TVertex> Vertices { get; }
  public GraphicsBuffer<uint>    Indices  { get; }

  public override Size Size => Vertices.Size + Indices.Size;

  public Tessellator<TVertex> CreateTessellator()
  {
    return new Tessellator<TVertex>();
  }

  public void Tessellate(Action<Tessellator<TVertex>> builder)
  {
    var tessellator = CreateTessellator();

    builder(tessellator);

    tessellator.WriteTo(this);
  }

  public override void Draw(Material material, MeshType type = MeshType.Triangles)
  {
    Draw(material, Vertices.Length, Indices.Length, type);
  }

  public override void Draw(Material material, int vertexCount, int indexCount, MeshType type = MeshType.Triangles)
  {
    material.Apply(server); // TODO: put this in a better place (material batching?)

    server.DrawMesh(
      mesh: Handle,
      vertexCount: vertexCount,
      indexCount: indexCount,
      meshType: type,
      indexType: Indices.ElementType
    );
  }

  public override void Draw(ShaderProgram shader, MeshType type = MeshType.Triangles)
  {
    Draw(shader, Vertices.Length, Indices.Length, type);
  }

  public override void Draw(ShaderProgram shader, int vertexCount, int indexCount, MeshType type = MeshType.Triangles)
  {
    server.SetActiveShader(shader.Handle);

    server.DrawMesh(
      mesh: Handle,
      vertexCount: vertexCount,
      indexCount: indexCount,
      meshType: type,
      indexType: Indices.ElementType
    );
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteMesh(Handle);

      Vertices.Dispose();
      Indices.Dispose();
    }

    base.Dispose(managed);
  }
}
