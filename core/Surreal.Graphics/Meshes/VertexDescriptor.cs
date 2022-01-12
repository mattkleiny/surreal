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
public readonly record struct VertexDescriptor(string Alias, int Count, VertexType Type, bool Normalized, int Offset)
{
  public int Stride => Count * DetermineSize(Type);

  private static int DetermineSize(VertexType type) => type switch
  {
    VertexType.Byte          => sizeof(byte),
    VertexType.UnsignedByte  => sizeof(byte),
    VertexType.Short         => sizeof(short),
    VertexType.UnsignedShort => sizeof(ushort),
    VertexType.Int           => sizeof(int),
    VertexType.UnsignedInt   => sizeof(uint),
    VertexType.Float         => sizeof(float),
    VertexType.Double        => sizeof(double),

    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
  };
}

/// <summary>Describes a set of <see cref="VertexDescriptor"/>s.</summary>
public sealed record VertexDescriptorSet(ImmutableArray<VertexDescriptor> Descriptors, int Stride)
{
  public int Length => Descriptors.Length;

  public VertexDescriptor this[int index] => Descriptors[index];

  public static VertexDescriptorSet Create<TVertex>()
    where TVertex : unmanaged
  {
    var values = typeof(TVertex)
      .GetFields(BindingFlags.Public | BindingFlags.Instance)
      .Where(member => member.GetCustomAttribute<VertexDescriptorAttribute>() != null)
      .Select(member => member.GetCustomAttribute<VertexDescriptorAttribute>()!);

    var builder = ImmutableArray.CreateBuilder<VertexDescriptor>();
    var stride  = 0;

    foreach (var attribute in (IEnumerable<VertexDescriptorAttribute>) values)
    {
      var descriptor = new VertexDescriptor(
        Alias: attribute.Alias,
        Count: attribute.Count,
        Type: attribute.Type,
        Normalized: attribute.Normalized,
        Offset: stride
      );

      stride += descriptor.Stride;

      builder.Add(descriptor);
    }

    return new VertexDescriptorSet(builder.ToImmutable(), stride);
  }
}
