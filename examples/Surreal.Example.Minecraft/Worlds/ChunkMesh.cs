using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Surreal.Diagnostics.Profiling;
using Surreal.Graphics;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics;

namespace Minecraft.Worlds;

public sealed class ChunkMesh : IDisposable
{
  private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<ChunkMesh>();

  private readonly Chunk        chunk;
  private readonly Mesh<Vertex> mesh;

  public ChunkMesh(IGraphicsDevice device, Chunk chunk)
  {
    this.chunk = chunk;

    mesh = new Mesh<Vertex>(device);

    chunk.Changed += OnChunkChanged;
  }

  public bool IsDirty { get; private set; } = true;
  public bool IsReady { get; private set; }

  public void Render(MeshType type)
  {
    if (IsDirty)
    {
      IsDirty = false;
      IsReady = false;

      Invalidate();
    }

    if (IsReady)
    {
      // TODO: draw the mesh
    }
  }

  private void OnChunkChanged()
  {
    IsDirty = true;
  }

  public Task Invalidate() => Task.Run(() =>
  {
    using var _ = Profiler.Track(nameof(Invalidate));

    var tessellator = new Tessellator();
    var voxels      = chunk.Voxels;

    // TODO: fix bounds checks
    for (var z = 1; z < chunk.Depth - 1; z++)
    for (var y = 1; y < chunk.Height - 1; y++)
    for (var x = 1; x < chunk.Width - 1; x++)
    {
      var voxel = voxels[x + y * chunk.Width + z * chunk.Width * chunk.Height];
      var block = chunk.Palette.GetBlock(voxel);

      if (!block.IsSolid) continue; // don't render geometry for non-solid blocks

      if (!chunk.GetBlock(x - 1, y, z).IsSolid) tessellator.AddFace(x, y, z, Face.Left, block.Color);
      if (!chunk.GetBlock(x + 1, y, z).IsSolid) tessellator.AddFace(x, y, z, Face.Right, block.Color);
      if (!chunk.GetBlock(x, y + 1, z).IsSolid) tessellator.AddFace(x, y, z, Face.Top, block.Color);
      if (!chunk.GetBlock(x, y - 1, z).IsSolid) tessellator.AddFace(x, y, z, Face.Bottom, block.Color);
      if (!chunk.GetBlock(x, y, z - 1).IsSolid) tessellator.AddFace(x, y, z, Face.Front, block.Color);
      if (!chunk.GetBlock(x, y, z + 1).IsSolid) tessellator.AddFace(x, y, z, Face.Back, block.Color);
    }

    Game.Schedule(() =>
    {
      // upload vertices/indices to the GPU
      mesh.Vertices.Write(tessellator.Vertices);
      mesh.Indices.Write(tessellator.Indices);

      IsReady = true;
    });
  });

  public void Dispose()
  {
    chunk.Changed -= OnChunkChanged;

    mesh.Dispose();
  }

  private enum Face
  {
    Left,
    Right,
    Top,
    Bottom,
    Front,
    Back,
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct Vertex
  {
    [VertexDescriptor(
      Alias = "a_position",
      Count = 3,
      Type = VertexType.Float
    )]
    public Vector3 Position;

    [VertexDescriptor(
      Alias = "a_normal",
      Count = 3,
      Type = VertexType.Float
    )]
    public Vector3 Normal;

    [VertexDescriptor(
      Alias = "a_color",
      Count = 4,
      Type = VertexType.UnsignedByte,
      Normalized = true
    )]
    public Color Color;

    [VertexDescriptor(
      Alias = "a_texCoord0",
      Count = 2,
      Type = VertexType.Float,
      Normalized = true
    )]
    public UV UV1;

    public Vertex(Vector3 position, Vector3 normal, Color color, UV uv1)
    {
      Position = position;
      Normal   = normal;
      Color    = color;
      UV1      = uv1;
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct UV
  {
    [UsedImplicitly] public float U;
    [UsedImplicitly] public float V;

    public UV(float u, float v)
    {
      U = u;
      V = v;
    }
  }

  private sealed class Tessellator
  {
    private readonly List<Vertex> vertices = new();
    private readonly List<ushort> indices  = new();

    public Span<Vertex> Vertices => CollectionsMarshal.AsSpan(vertices);
    public Span<ushort> Indices  => CollectionsMarshal.AsSpan(indices);

    public void AddFace(int x, int y, int z, Face face, Color color)
    {
      const float size = 0.5f;

      indices.Add((ushort) (vertices.Count + 0));
      indices.Add((ushort) (vertices.Count + 1));
      indices.Add((ushort) (vertices.Count + 2));
      indices.Add((ushort) (vertices.Count + 0));
      indices.Add((ushort) (vertices.Count + 2));
      indices.Add((ushort) (vertices.Count + 3));

      switch (face)
      {
        case Face.Left:
          vertices.Add(new Vertex(new(x - size, y - size, z + size), new(-1f, 0f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y + size, z + size), new(-1f, 0f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y + size, z - size), new(-1f, 0f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y - size, z - size), new(-1f, 0f, 0f), color, new UV(0f, 0f)));
          break;

        case Face.Right:
          vertices.Add(new Vertex(new(x + size, y - size, z - size), new(1f, 0f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z - size), new(1f, 0f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z + size), new(1f, 0f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y - size, z + size), new(1f, 0f, 0f), color, new UV(0f, 0f)));
          break;

        case Face.Top:
          vertices.Add(new Vertex(new(x - size, y + size, z + size), new(0f, 1f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z + size), new(0f, 1f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z - size), new(0f, 1f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y + size, z - size), new(0f, 1f, 0f), color, new UV(0f, 0f)));
          break;

        case Face.Bottom:
          vertices.Add(new Vertex(new(x - size, y - size, z + size), new(0f, -1f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y - size, z - size), new(0f, -1f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y - size, z - size), new(0f, -1f, 0f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y - size, z + size), new(0f, -1f, 0f), color, new UV(0f, 0f)));
          break;

        case Face.Front:
          vertices.Add(new Vertex(new(x - size, y - size, z - size), new(0f, 0f, -1f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y + size, z - size), new(0f, 0f, -1f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z - size), new(0f, 0f, -1f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y - size, z - size), new(0f, 0f, -1f), color, new UV(0f, 0f)));
          break;

        case Face.Back:
          vertices.Add(new Vertex(new(x - size, y + size, z + size), new(0f, 0f, 1f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y - size, z + size), new(0f, 0f, 1f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y - size, z + size), new(0f, 0f, 1f), color, new UV(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z + size), new(0f, 0f, 1f), color, new UV(0f, 0f)));
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(face), face, "An unrecognized face was requested.");
      }
    }
  }
}
