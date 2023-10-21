using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Shaders;
using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

/// <summary>
/// Different types of supported primitive shapes for meshes.
/// </summary>
public enum MeshType
{
  Points,
  Lines,
  LineStrip,
  LineLoop,
  Triangles
}

/// <summary>
/// Abstracts over all possible <see cref="Mesh{TVertex}" /> types.
/// </summary>
public abstract class Mesh : GraphicsAsset, IHasSizeEstimate
{
  public abstract Size Size { get; }

  /// <summary>
  /// Convenience method for building <see cref="Mesh{TVertex}" />s with a <see cref="Tessellator{TVertex}" />.
  /// </summary>
  public static Mesh<TVertex> Create<TVertex>(GraphicsContext context, Action<Tessellator<TVertex>> builder)
    where TVertex : unmanaged
  {
    var mesh = new Mesh<TVertex>(context);

    mesh.Tessellate(builder);

    return mesh;
  }

  /// <summary>
  /// Builds a simple triangle <see cref="Mesh" />.
  /// </summary>
  public static Mesh<Vertex2> CreateTriangle(GraphicsContext context, float size = 1f)
  {
    return Create<Vertex2>(context, tessellator =>
    {
      tessellator.AddTriangle(
        new Vertex2(new Vector2(-size, -size), Color.White, new Vector2(0f, 0f)),
        new Vertex2(new Vector2(0f, size), Color.White, new Vector2(0.5f, 1f)),
        new Vertex2(new Vector2(size, -size), Color.White, new Vector2(1f, 0f))
      );
    });
  }

  /// <summary>
  /// Builds a simple quad <see cref="Mesh" /> quad.
  /// </summary>
  public static Mesh<Vertex2> CreateQuad(GraphicsContext context, float size = 1f)
  {
    return Create<Vertex2>(context, tessellator =>
    {
      tessellator.AddQuad(
        new Vertex2(new Vector2(-size, -size), Color.White, new Vector2(0f, 1f)),
        new Vertex2(new Vector2(-size, size), Color.White, new Vector2(0f, 0f)),
        new Vertex2(new Vector2(size, size), Color.White, new Vector2(1f, 0f)),
        new Vertex2(new Vector2(size, -size), Color.White, new Vector2(1f, 1f))
      );
    });
  }

  /// <summary>
  /// Builds a simple circle <see cref="Mesh" />.
  /// </summary>
  public static Mesh<Vertex2> CreateCircle(GraphicsContext context, float radius = 1f, int segments = 16)
  {
    return Create<Vertex2>(context, tessellator =>
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

        vertices.Add(new Vertex2(new Vector2(x, y), Color.White, new Vector2(u, v)));
      }

      tessellator.AddTriangleFan(vertices);
    });
  }

  /// <summary>
  /// Draws the mesh with the given <see cref="ShaderProgram" />.
  /// </summary>
  public abstract void Draw(ShaderProgram shader, MeshType type = MeshType.Triangles);

  /// <summary>
  /// Draws the mesh with the given <see cref="ShaderProgram" /> and primitive counts.
  /// </summary>
  public abstract void Draw(ShaderProgram shader, uint vertexCount, uint indexCount, MeshType type = MeshType.Triangles);

  /// <summary>
  /// Draws the mesh with the given <see cref="Material" />.
  /// </summary>
  public abstract void Draw(Material material, MeshType type = MeshType.Triangles);

  /// <summary>
  /// Draws the mesh with the given <see cref="Material" /> and primitive counts.
  /// </summary>
  public abstract void Draw(Material material, uint vertexCount, uint indexCount, MeshType type = MeshType.Triangles);
}

/// <summary>
/// A mesh with a strongly-typed vertex type, <see cref="TVertex" />.
/// </summary>
[DebuggerDisplay("Mesh with {Vertices.Length} vertices and {Indices.Length} indices")]
public sealed class Mesh<TVertex> : Mesh
  where TVertex : unmanaged
{
  /// <summary>
  /// Descriptors are statically shared amongst all <see cref="TVertex" />.
  /// </summary>
  private static readonly VertexDescriptorSet VertexDescriptors = VertexDescriptorSet.Create<TVertex>();

  private readonly GraphicsContext _context;

  public Mesh(GraphicsContext context, BufferUsage usage = BufferUsage.Static)
  {
    _context = context;

    Vertices = new GraphicsBuffer<TVertex>(context, BufferType.Vertex, usage);
    Indices = new GraphicsBuffer<uint>(context, BufferType.Index, usage);

    Handle = context.Backend.CreateMesh(Vertices.Handle, Indices.Handle, VertexDescriptors);
  }

  public GraphicsHandle Handle { get; }
  public GraphicsBuffer<TVertex> Vertices { get; }
  public GraphicsBuffer<uint> Indices { get; }

  public override Size Size => Vertices.Size + Indices.Size;

  public void Tessellate(Action<Tessellator<TVertex>> builder)
  {
    var tessellator = new Tessellator<TVertex>();

    builder(tessellator);

    tessellator.WriteTo(this);
  }

  public override void Draw(Material material, MeshType type = MeshType.Triangles)
  {
    Draw(material, Vertices.Length, Indices.Length, type);
  }

  public override void Draw(Material material, uint vertexCount, uint indexCount, MeshType type = MeshType.Triangles)
  {
    material.Apply(_context); // TODO: put this in a better place (material batching?)

    _context.Backend.DrawMesh(
      Handle,
      vertexCount,
      indexCount,
      type,
      Indices.ElementType
    );
  }

  public override void Draw(ShaderProgram shader, MeshType type = MeshType.Triangles)
  {
    Draw(shader, Vertices.Length, Indices.Length, type);
  }

  public override void Draw(ShaderProgram shader, uint vertexCount, uint indexCount, MeshType type = MeshType.Triangles)
  {
    _context.Backend.SetActiveShader(shader.Handle);

    _context.Backend.DrawMesh(
      Handle,
      vertexCount,
      indexCount,
      type,
      Indices.ElementType
    );
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      _context.Backend.DeleteMesh(Handle);

      Vertices.Dispose();
      Indices.Dispose();
    }

    base.Dispose(managed);
  }
}
