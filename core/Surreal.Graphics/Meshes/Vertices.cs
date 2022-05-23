using System.Runtime.InteropServices;
using Surreal.Mathematics;

namespace Surreal.Graphics.Meshes;

/// <summary>A common 2d vertex type for primitive shapes.</summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Vertex2(Vector2 Position, Color Color, Vector2 UV)
{
  [VertexDescriptor(2, VertexType.Float)]
  public Vector2 Position = Position;

  [VertexDescriptor(4, VertexType.Float)]
  public Color Color = Color;

  [VertexDescriptor(2, VertexType.Float)]
  public Vector2 UV = UV;
}

/// <summary>A common 3d vertex type for primitive shapes.</summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Vertex3(Vector3 Position, Color Color, Vector2 UV)
{
  [VertexDescriptor(3, VertexType.Float)]
  public Vector3 Position = Position;

  [VertexDescriptor(4, VertexType.Float)]
  public Color Color = Color;

  [VertexDescriptor(2, VertexType.Float)]
  public Vector2 UV = UV;
}
