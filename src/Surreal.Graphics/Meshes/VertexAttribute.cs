using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Surreal.Graphics.Meshes {
  [MeansImplicitUse]
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class VertexAttributeAttribute : Attribute {
    public string     Alias      { get; set; } = string.Empty;
    public int        Count      { get; set; }
    public VertexType Type       { get; set; }
    public bool       Normalized { get; set; }
  }

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

  public sealed class VertexAttributeSet {
    private readonly VertexAttribute[] attributes;

    public static VertexAttributeSet Create<TVertex>()
        where TVertex : unmanaged => new VertexAttributeSet(typeof(TVertex)
        .GetMembers(BindingFlags.Public | BindingFlags.Instance)
        .OfType<FieldInfo>()
        .Where(member => member.GetCustomAttribute<VertexAttributeAttribute>() != null)
        .Select(member => member.GetCustomAttribute<VertexAttributeAttribute>())
        .ToArray());

    private VertexAttributeSet(params VertexAttributeAttribute[] attributes) {
      Debug.Assert(attributes.Length > 0, "At least one attribute must be specified.");

      this.attributes = CreateAttributes(attributes).ToArray();
    }

    public int Length => attributes.Length;
    public int Stride { get; private set; }

    public VertexAttribute this[int index] => attributes[index];

    public override string ToString() => string.Join(", ", attributes.Select(it => it.ToString()).ToArray());

    private IEnumerable<VertexAttribute> CreateAttributes(IEnumerable<VertexAttributeAttribute> attributes) {
      var accumulator = 0;

      foreach (var attribute in attributes) {
        var result = new VertexAttribute(
            alias: attribute.Alias,
            count: attribute.Count,
            type: attribute.Type,
            normalized: attribute.Normalized,
            offset: accumulator
        );

        accumulator += result.Stride;

        yield return result;
      }

      // calculate total stride of the members
      Stride = accumulator;
    }
  }
}