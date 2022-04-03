using Surreal.Graphics.Meshes;

namespace Perimeter.Core;

/// <summary>Manages the underlying mesh of world geometry for renedering.</summary>
public sealed class WorldMesh : IDisposable
{
  private readonly Mesh<Vertex> mesh;
  private readonly PriorityQueue<DirtyVolume, float> dirtyVolumes = new();

  public WorldMesh(IGraphicsServer server)
  {
    mesh = new Mesh<Vertex>(server);
  }

  public Mesh Mesh => mesh;

  public void MarkDirty(Vector3 center, Vector3 size, float priority = 0f)
  {
    var segment = new DirtyVolume(center, size);

    dirtyVolumes.Enqueue(segment, priority);
  }

  public void Update()
  {
    if (dirtyVolumes.TryDequeue(out var volume, out _))
    {
      // process one segment each frame, constrained by natural priority
      RebuildDirtyVolume(volume);
    }

    throw new NotImplementedException();
  }

  public void Invalidate()
  {
    // invalidates the entire map
    throw new NotImplementedException();
  }

  private void RebuildDirtyVolume(DirtyVolume volume)
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
    mesh.Dispose();
  }

  public record struct Vertex(Vector2 Position, Vector2 UV, Color Color)
  {
    [VertexDescriptor(
      Count = 2,
      Type = VertexType.Float
    )]
    public Vector2 Position = Position;

    [VertexDescriptor(
      Count = 2,
      Type = VertexType.Float,
      Normalized = true
    )]
    public Vector2 UV = UV;

    [VertexDescriptor(
      Count = 4,
      Type = VertexType.UnsignedByte,
      Normalized = true
    )]
    public Color Color = Color;
  }

  private readonly record struct DirtyVolume(Vector3 Center, Vector3 Size);
}
