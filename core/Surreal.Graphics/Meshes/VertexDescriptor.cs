using System.Reflection;
using JetBrains.Annotations;

namespace Surreal.Graphics.Meshes;

#pragma warning disable CA1720

/// <summary>Primitive types supported by <see cref="VertexDescriptor"/>s.</summary>
public enum VertexType
{
  Byte,
  UnsignedByte,
  Short,
  UnsignedShort,
  Int,
  UnsignedInt,
  Float,
  Double,
}

/// <summary>Associates a <see cref="VertexDescriptor"/> with a vertex field.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field)]
public sealed class VertexDescriptorAttribute : Attribute
{
  public string     Alias      { get; set; } = string.Empty;
  public int        Count      { get; set; }
  public VertexType Type       { get; set; }
  public bool       Normalized { get; set; }
}

/// <summary>Describes a single vertex.</summary>
[DebuggerDisplay("{Alias}: {Count}")]
public readonly struct VertexDescriptor
{
  public readonly string     Alias;
  public readonly int        Count;
  public readonly int        Size;
  public readonly bool       Normalized;
  public readonly VertexType Type;
  public readonly int        Offset;

  public VertexDescriptor(string alias, int count, VertexType type, bool normalized, int offset)
  {
    Normalized = normalized;
    Alias      = alias;
    Count      = count;
    Size       = DetermineSize(type);
    Type       = type;
    Offset     = offset;
  }

  public int Stride => Size * Count;

  private static int DetermineSize(VertexType type)
  {
    switch (type)
    {
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

/// <summary>Describes a set of <see cref="VertexDescriptor"/>s.</summary>
public sealed class VertexDescriptorSet
{
  private readonly ImmutableArray<VertexDescriptor> attributes;

  public static VertexDescriptorSet Create<TVertex>()
    where TVertex : unmanaged
  {
    var values = typeof(TVertex)
      .GetMembers(BindingFlags.Public | BindingFlags.Instance)
      .OfType<FieldInfo>()
      .Where(member => member.GetCustomAttribute<VertexDescriptorAttribute>() != null)
      .Select(member => member.GetCustomAttribute<VertexDescriptorAttribute>()!);

    return new VertexDescriptorSet(values);
  }

  private VertexDescriptorSet(IEnumerable<VertexDescriptorAttribute> attributes)
  {
    this.attributes = CreateAttributes(attributes);
  }

  public int Length => attributes.Length;
  public int Stride { get; private set; }

  public VertexDescriptor this[int index] => attributes[index];

  public override string ToString()
  {
    return string.Join(", ", attributes.Select(it => it.ToString()).ToArray());
  }

  private ImmutableArray<VertexDescriptor> CreateAttributes(IEnumerable<VertexDescriptorAttribute> attributes)
  {
    var builder = ImmutableArray.CreateBuilder<VertexDescriptor>();
    var stride  = 0;

    foreach (var attribute in attributes)
    {
      var descriptor = new VertexDescriptor(
        alias: attribute.Alias,
        count: attribute.Count,
        type: attribute.Type,
        normalized: attribute.Normalized,
        offset: stride
      );

      stride += descriptor.Stride;

      builder.Add(descriptor);
    }

    // calculate total stride of the members
    Stride = stride;

    return builder.ToImmutable();
  }
}
