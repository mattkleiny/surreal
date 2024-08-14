using Surreal.Colors;

namespace Surreal.Graphics.Meshes;

/// <summary>
/// A common 2d vertex type for primitive shapes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Vertex2(Vector2 Position, Color32 Color, Vector2 UV)
{
  [VertexDescriptor(2, VertexType.Float)]
  public Vector2 Position = Position;

  [VertexDescriptor(2, VertexType.Float)]
  public Vector2 UV = UV;

  [VertexDescriptor(4, VertexType.UnsignedByte, ShouldNormalize = true)]
  public Color32 Color = Color;
}
