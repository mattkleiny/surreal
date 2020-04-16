using System.Numerics;
using System.Runtime.InteropServices;
using Surreal.Graphics.Meshes;

namespace Surreal.Graphics
{
  [StructLayout(LayoutKind.Sequential)]
  internal struct Vertex
  {
    [VertexAttribute(
      Alias = "a_position",
      Count = 3,
      Type  = VertexType.Float
    )]
    public Vector3 Position;

    [VertexAttribute(
      Alias      = "a_color",
      Count      = 4,
      Type       = VertexType.UnsignedByte,
      Normalized = true
    )]
    public Color Color;
  }
}
