using System.Reflection;
using JetBrains.Annotations;

namespace Surreal.Graphics.Meshes;

/// <summary>Primitive types supported by <see cref="VertexDescriptor"/>s.</summary>
public enum VertexType
{
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
  public string?    Alias { get; set; }
  public int        Count { get; set; } = 4;
  public VertexType Type  { get; set; } = VertexType.Float;

  /// <summary>True if the resultant components should be normalised to (0, 1) before submission to the GPU.</summary>
  public bool ShouldNormalize { get; set; } = false;
}

/// <summary>Describes a single vertex.</summary>
[DebuggerDisplay("{Alias}: {Count}")]
public readonly record struct VertexDescriptor(int Offset, string Alias, int Count, VertexType Type, bool ShouldNormalize)
{
  public int Stride => Count * DetermineSize(Type);

  private static int DetermineSize(VertexType type) => type switch
  {
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
      .Select(member => (member.Name, member.GetCustomAttribute<VertexDescriptorAttribute>()!));

    var builder = ImmutableArray.CreateBuilder<VertexDescriptor>();
    var stride = 0;

    foreach (var (name, attribute) in values)
    {
      var descriptor = new VertexDescriptor(
        Offset: stride,
        Alias: attribute.Alias ?? name,
        Count: attribute.Count,
        Type: attribute.Type,
        ShouldNormalize: attribute.ShouldNormalize
      );

      stride += descriptor.Stride;

      builder.Add(descriptor);
    }

    return new VertexDescriptorSet(builder.ToImmutable(), stride);
  }
}
