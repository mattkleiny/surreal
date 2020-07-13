using System;
using System.Diagnostics;

namespace Surreal.Graphics.Meshes {
  [DebuggerDisplay("{Alias}: {Count}")]
  public readonly struct VertexAttribute {
    public readonly string     Alias;
    public readonly int        Count;
    public readonly int        Size;
    public readonly bool       Normalized;
    public readonly VertexType Type;
    public readonly int        Offset;

    public int Stride => Size * Count;

    public VertexAttribute(string alias, int count, VertexType type, bool normalized, int offset) {
      Normalized = normalized;
      Alias      = alias;
      Count      = count;
      Size       = DetermineSize(type);
      Type       = type;
      Offset     = offset;
    }

    private static int DetermineSize(VertexType type) {
      switch (type) {
        case VertexType.Byte:          return sizeof(byte);
        case VertexType.UnsignedByte:  return sizeof(byte);
        case VertexType.Short:         return sizeof(short);
        case VertexType.UnsignedShort: return sizeof(ushort);
        case VertexType.Int:           return sizeof(int);
        case VertexType.UnsignedInt:   return sizeof(uint);
        case VertexType.Float:         return sizeof(float);
        case VertexType.Double:        return sizeof(double);

        default:
          throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
    }
  }
}