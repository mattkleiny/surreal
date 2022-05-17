using System.Runtime.InteropServices;
using Surreal.Diagnostics.Profiling;

namespace Minecraft.Worlds;

/// <summary>A triangle mesh constructed from a source <see cref="Chunk"/>.</summary>
public sealed class ChunkMesh : IDisposable
{
  private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<ChunkMesh>();

  private readonly Chunk chunk;
  private readonly Mesh<Vertex> mesh;

  public ChunkMesh(IGraphicsServer server, Chunk chunk)
  {
    this.chunk = chunk;

    mesh = new Mesh<Vertex>(server);

    chunk.Changed += OnChunkChanged;
  }

  public bool IsDirty { get; private set; } = true;
  public bool IsReady { get; private set; }

  public void Draw(Material material)
  {
    if (IsDirty)
    {
      IsDirty = false;
      IsReady = false;

      Task.Run(Invalidate); // invalidate on background thread
    }

    if (IsReady)
    {
      mesh.Draw(material);
    }
  }

  private void OnChunkChanged()
  {
    IsDirty = true;
  }

  /// <summary>Synchronously rebuilds the underlying mesh geometry.</summary>
  public void Invalidate()
  {
    using var _ = Profiler.Track(nameof(Invalidate));

    var tessellator = new Tessellator();

    // TODO: fix bounds checks
    for (var z = 1; z < chunk.Depth - 1; z++)
    for (var y = 1; y < chunk.Height - 1; y++)
    for (var x = 1; x < chunk.Width - 1; x++)
    {
      var block = chunk.GetBlock(x, y, z);
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
  }

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
  private record struct Vertex(Vector3 Position, Vector3 Normal, Color Color, Vector2 UV1)
  {
    [VertexDescriptor(VertexType.Float, 3)]
    public Vector3 Position = Position;

    [VertexDescriptor(VertexType.Float, 3)]
    public Vector3 Normal = Normal;

    [VertexDescriptor(VertexType.Float, 4)]
    public Color Color = Color;

    [VertexDescriptor(VertexType.Float, 2)]
    public Vector2 UV1 = UV1;
  }

  private sealed class Tessellator
  {
    private readonly List<Vertex> vertices = new();
    private readonly List<uint> indices = new();

    public Span<Vertex> Vertices => vertices.AsSpan();
    public Span<uint>   Indices  => indices.AsSpan();

    public void AddFace(int x, int y, int z, Face face, Color color)
    {
      const float size = 0.5f;

      indices.Add((uint) (vertices.Count + 0));
      indices.Add((uint) (vertices.Count + 1));
      indices.Add((uint) (vertices.Count + 2));
      indices.Add((uint) (vertices.Count + 0));
      indices.Add((uint) (vertices.Count + 2));
      indices.Add((uint) (vertices.Count + 3));

      switch (face)
      {
        case Face.Left:
          vertices.Add(new Vertex(new(x - size, y - size, z + size), new(-1f, 0f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y + size, z + size), new(-1f, 0f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y + size, z - size), new(-1f, 0f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y - size, z - size), new(-1f, 0f, 0f), color, new(0f, 0f)));
          break;

        case Face.Right:
          vertices.Add(new Vertex(new(x + size, y - size, z - size), new(1f, 0f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z - size), new(1f, 0f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z + size), new(1f, 0f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y - size, z + size), new(1f, 0f, 0f), color, new(0f, 0f)));
          break;

        case Face.Top:
          vertices.Add(new Vertex(new(x - size, y + size, z + size), new(0f, 1f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z + size), new(0f, 1f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z - size), new(0f, 1f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y + size, z - size), new(0f, 1f, 0f), color, new(0f, 0f)));
          break;

        case Face.Bottom:
          vertices.Add(new Vertex(new(x - size, y - size, z + size), new(0f, -1f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y - size, z - size), new(0f, -1f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y - size, z - size), new(0f, -1f, 0f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y - size, z + size), new(0f, -1f, 0f), color, new(0f, 0f)));
          break;

        case Face.Front:
          vertices.Add(new Vertex(new(x - size, y - size, z - size), new(0f, 0f, -1f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y + size, z - size), new(0f, 0f, -1f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z - size), new(0f, 0f, -1f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y - size, z - size), new(0f, 0f, -1f), color, new(0f, 0f)));
          break;

        case Face.Back:
          vertices.Add(new Vertex(new(x - size, y + size, z + size), new(0f, 0f, 1f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x - size, y - size, z + size), new(0f, 0f, 1f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y - size, z + size), new(0f, 0f, 1f), color, new(0f, 0f)));
          vertices.Add(new Vertex(new(x + size, y + size, z + size), new(0f, 0f, 1f), color, new(0f, 0f)));
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(face), face, "An unrecognized face was requested.");
      }
    }
  }
}
