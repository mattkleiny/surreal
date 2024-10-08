﻿using Surreal.Colors;
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
public abstract class Mesh : Disposable
{
  /// <summary>
  /// The byte size of the <see cref="Mesh" />.
  /// </summary>
  public abstract Size Size { get; }

  /// <summary>
  /// Builds a simple triangle <see cref="Mesh" />.
  /// </summary>
  public static Mesh<Vertex2> CreateTriangle(IGraphicsDevice device, float size = 1f)
  {
    var tessellator = new MeshTessellator<Vertex2>();

    tessellator.AddTriangle(
      new Vertex2(new Vector2(-size, -size), Color.White, new Vector2(0f, 0f)),
      new Vertex2(new Vector2(0f, size), Color.White, new Vector2(0.5f, 1f)),
      new Vertex2(new Vector2(size, -size), Color.White, new Vector2(1f, 0f))
    );

    return tessellator.ToMesh(device);
  }

  /// <summary>
  /// Builds a simple quad <see cref="Mesh" /> quad.
  /// </summary>
  public static Mesh<Vertex2> CreateQuad(IGraphicsDevice device, float size = 1f)
  {
    var tessellator = new MeshTessellator<Vertex2>();

    tessellator.AddQuad(
      new Vertex2(new Vector2(-size, -size), Color.White, new Vector2(0f, 1f)),
      new Vertex2(new Vector2(-size, size), Color.White, new Vector2(0f, 0f)),
      new Vertex2(new Vector2(size, size), Color.White, new Vector2(1f, 0f)),
      new Vertex2(new Vector2(size, -size), Color.White, new Vector2(1f, 1f))
    );

    return tessellator.ToMesh(device);
  }

  /// <summary>
  /// Builds a simple circle <see cref="Mesh" />.
  /// </summary>
  public static Mesh<Vertex2> CreateCircle(IGraphicsDevice device, float radius = 1f, int segments = 16)
  {
    var tessellator = new MeshTessellator<Vertex2>();

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

    return tessellator.ToMesh(device);
  }

  /// <summary>
  /// Builds a simple cube <see cref="Mesh" />.
  /// </summary>
  public static Mesh<Vertex3> CreateCube(IGraphicsDevice device, float size = 1f)
  {
    var tessellator = new MeshTessellator<Vertex3>();

    var vertices = new SpanList<Vertex3>(stackalloc Vertex3[6]);

    // front
    vertices.Add(new Vertex3(new Vector3(-size, -size, size), Color.White, new Vector2(0f, 0f)));
    vertices.Add(new Vertex3(new Vector3(-size, size, size), Color.White, new Vector2(0f, 1f)));
    vertices.Add(new Vertex3(new Vector3(size, size, size), Color.White, new Vector2(1f, 1f)));
    vertices.Add(new Vertex3(new Vector3(size, -size, size), Color.White, new Vector2(1f, 0f)));

    tessellator.AddTriangleFan(vertices);
    vertices.Clear();

    // back
    vertices.Add(new Vertex3(new Vector3(size, -size, -size), Color.White, new Vector2(0f, 0f)));
    vertices.Add(new Vertex3(new Vector3(size, size, -size), Color.White, new Vector2(0f, 1f)));
    vertices.Add(new Vertex3(new Vector3(-size, size, -size), Color.White, new Vector2(1f, 1f)));
    vertices.Add(new Vertex3(new Vector3(-size, -size, -size), Color.White, new Vector2(1f, 0f)));

    tessellator.AddTriangleFan(vertices);
    vertices.Clear();

    // left
    vertices.Add(new Vertex3(new Vector3(-size, -size, -size), Color.White, new Vector2(0f, 0f)));
    vertices.Add(new Vertex3(new Vector3(-size, size, -size), Color.White, new Vector2(0f, 1f)));
    vertices.Add(new Vertex3(new Vector3(-size, size, size), Color.White, new Vector2(1f, 1f)));
    vertices.Add(new Vertex3(new Vector3(-size, -size, size), Color.White, new Vector2(1f, 0f)));

    tessellator.AddTriangleFan(vertices);
    vertices.Clear();

    // right
    vertices.Add(new Vertex3(new Vector3(size, -size, size), Color.White, new Vector2(0f, 0f)));
    vertices.Add(new Vertex3(new Vector3(size, size, size), Color.White, new Vector2(0f, 1f)));
    vertices.Add(new Vertex3(new Vector3(size, size, -size), Color.White, new Vector2(1f, 1f)));
    vertices.Add(new Vertex3(new Vector3(size, -size, -size), Color.White, new Vector2(1f, 0f)));

    tessellator.AddTriangleFan(vertices);
    vertices.Clear();

    // top
    vertices.Add(new Vertex3(new Vector3(-size, size, size), Color.White, new Vector2(0f, 0f)));
    vertices.Add(new Vertex3(new Vector3(-size, size, -size), Color.White, new Vector2(0f, 1f)));
    vertices.Add(new Vertex3(new Vector3(size, size, -size), Color.White, new Vector2(1f, 1f)));
    vertices.Add(new Vertex3(new Vector3(size, size, size), Color.White, new Vector2(1f, 0f)));

    tessellator.AddTriangleFan(vertices);
    vertices.Clear();

    // bottom
    vertices.Add(new Vertex3(new Vector3(-size, -size, -size), Color.White, new Vector2(0f, 0f)));
    vertices.Add(new Vertex3(new Vector3(-size, -size, size), Color.White, new Vector2(0f, 1f)));
    vertices.Add(new Vertex3(new Vector3(size, -size, size), Color.White, new Vector2(1f, 1f)));
    vertices.Add(new Vertex3(new Vector3(size, -size, -size), Color.White, new Vector2(1f, 0f)));

    return tessellator.ToMesh(device);
  }

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

  private readonly IGraphicsDevice _device;

  public Mesh(IGraphicsDevice device, BufferUsage usage = BufferUsage.Static)
  {
    _device = device;

    Vertices = new GraphicsBuffer<TVertex>(device, BufferType.Vertex, usage);
    Indices = new GraphicsBuffer<uint>(device, BufferType.Index, usage);

    Handle = device.CreateMesh(Vertices.Handle, Indices.Handle, VertexDescriptors);
  }

  /// <summary>
  /// The <see cref="GraphicsHandle"/> for the mesh itself.
  /// </summary>
  public GraphicsHandle Handle { get; }

  /// <summary>
  /// The <see cref="GraphicsBuffer" /> for the vertices.
  /// </summary>
  public GraphicsBuffer<TVertex> Vertices { get; }

  /// <summary>
  /// The <see cref="GraphicsBuffer" /> for the indices.
  /// </summary>
  public GraphicsBuffer<uint> Indices { get; }

  /// <summary>
  /// The <see cref="VertexDescriptorSet" /> for the mesh.
  /// </summary>
  public VertexDescriptorSet Descriptors => VertexDescriptors;

  /// <inheritdoc/>
  public override Size Size => Vertices.Size + Indices.Size;

  /// <inheritdoc/>
  public override void Draw(Material material, MeshType type = MeshType.Triangles)
  {
    Draw(material, Vertices.Length, Indices.Length, type);
  }

  /// <inheritdoc/>
  public override void Draw(Material material, uint vertexCount, uint indexCount, MeshType type = MeshType.Triangles)
  {
    material.ApplyMaterial();

    _device.DrawMesh(
      mesh: Handle,
      vertexCount: vertexCount,
      indexCount: indexCount,
      meshType: type,
      indexType: Indices.DataType
    );
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      _device.DeleteMesh(Handle);

      Vertices.Dispose();
      Indices.Dispose();
    }

    base.Dispose(managed);
  }
}
