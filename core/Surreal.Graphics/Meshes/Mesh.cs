using Surreal.Colors;
using Surreal.Graphics.Materials;
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
  /// <summary>
  /// The byte size of the <see cref="Mesh" />.
  /// </summary>
  public abstract Size Size { get; }

  /// <summary>
  /// Convenience method for building <see cref="Mesh{TVertex}" />s with a <see cref="Tessellator{TVertex}" />.
  /// </summary>
  public static Mesh<TVertex> Create<TVertex>(IGraphicsBackend backend, Action<Tessellator<TVertex>> builder)
    where TVertex : unmanaged
  {
    var mesh = new Mesh<TVertex>(backend);

    mesh.Tessellate(builder);

    return mesh;
  }

  /// <summary>
  /// Builds a simple triangle <see cref="Mesh" />.
  /// </summary>
  public static Mesh<Vertex2> CreateTriangle(IGraphicsBackend backend, float size = 1f)
  {
    return Create<Vertex2>(backend, tessellator =>
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
  public static Mesh<Vertex2> CreateQuad(IGraphicsBackend backend, float size = 1f)
  {
    return Create<Vertex2>(backend, tessellator =>
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
  public static Mesh<Vertex2> CreateCircle(IGraphicsBackend backend, float radius = 1f, int segments = 16)
  {
    return Create<Vertex2>(backend, tessellator =>
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

  private readonly IGraphicsBackend _backend;

  public Mesh(IGraphicsBackend backend, BufferUsage usage = BufferUsage.Static)
  {
    _backend = backend;

    Vertices = new GraphicsBuffer<TVertex>(backend, BufferType.Vertex, usage);
    Indices = new GraphicsBuffer<uint>(backend, BufferType.Index, usage);

    Handle = backend.CreateMesh(Vertices.Handle, Indices.Handle, VertexDescriptors);
  }

  /// <summary>
  /// The <see cref="GraphicsHandle"/> for the mesh itself.
  /// </summary>
  public GraphicsHandle Handle { get; }

  /// <summary>
  /// The <see cref="GraphicsBuffer{TVertex}" /> for the vertices.
  /// </summary>
  public GraphicsBuffer<TVertex> Vertices { get; }

  /// <summary>
  /// The <see cref="GraphicsBuffer{uint}" /> for the indices.
  /// </summary>
  public GraphicsBuffer<uint> Indices { get; }

  /// <summary>
  /// The <see cref="VertexDescriptorSet" /> for the mesh.
  /// </summary>
  public VertexDescriptorSet Descriptors => VertexDescriptors;

  /// <inheritdoc/>
  public override Size Size => Vertices.Size + Indices.Size;

  /// <summary>
  /// Tessellates a shape given by a <see cref="Tessellator{TVertex}" /> into this <see cref="Mesh{TVertex}" />.
  /// </summary>
  public void Tessellate(Action<Tessellator<TVertex>> builder)
  {
    var tessellator = new Tessellator<TVertex>();

    builder(tessellator);

    tessellator.WriteTo(this);
  }

  /// <inheritdoc/>
  public override void Draw(Material material, MeshType type = MeshType.Triangles)
  {
    Draw(material, Vertices.Length, Indices.Length, type);
  }

  /// <inheritdoc/>
  public override void Draw(Material material, uint vertexCount, uint indexCount, MeshType type = MeshType.Triangles)
  {
    material.Apply(_backend); // TODO: put this in a better place (material batching?)

    _backend.DrawMesh(
      mesh: Handle,
      vertexCount: vertexCount,
      indexCount: indexCount,
      meshType: type,
      indexType: Indices.ElementType
    );
  }

  /// <inheritdoc/>
  public override void Draw(ShaderProgram shader, MeshType type = MeshType.Triangles)
  {
    Draw(shader, Vertices.Length, Indices.Length, type);
  }

  /// <inheritdoc/>
  public override void Draw(ShaderProgram shader, uint vertexCount, uint indexCount, MeshType type = MeshType.Triangles)
  {
    _backend.SetActiveShader(shader.Handle);

    _backend.DrawMesh(
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
      _backend.DeleteMesh(Handle);

      Vertices.Dispose();
      Indices.Dispose();
    }

    base.Dispose(managed);
  }
}
