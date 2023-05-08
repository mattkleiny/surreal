using Surreal.Collections;

namespace Surreal.Graphics.Meshes;

/// <summary>
/// A utility for tessellating shapes into meshes of vertex geometry.
/// </summary>
public sealed class Tessellator<TVertex>
  where TVertex : unmanaged
{
  private readonly List<uint> _indices = new();
  private readonly List<TVertex> _vertices = new();

  public uint VertexCount => (uint)_vertices.Count;
  public uint IndexCount => (uint)_indices.Count;

  public ReadOnlySpan<TVertex> Vertices => _vertices.AsSpan();
  public ReadOnlySpan<uint> Indices => _indices.AsSpan();

  public void AddVertex(TVertex vertex)
  {
    _vertices.Add(vertex);
  }

  public void AddIndex(uint index)
  {
    _indices.Add((ushort)index);
  }

  public void Clear()
  {
    _vertices.Clear();
    _indices.Clear();
  }

  public void WriteTo(Mesh<TVertex> mesh)
  {
    mesh.Vertices.Write(Vertices);
    mesh.Indices.Write(Indices);
  }
}
