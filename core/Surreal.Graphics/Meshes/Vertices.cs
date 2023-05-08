using System.Reflection;
using System.Runtime.InteropServices;
using Surreal.Colors;

namespace Surreal.Graphics.Meshes;

/// <summary>
/// A common 2d vertex type for primitive shapes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Vertex2(Vector2 Position, ColorF Color, Vector2 UV)
{
  [VertexDescriptor(4, VertexType.Float)]
  public ColorF Color = Color;

  [VertexDescriptor(2, VertexType.Float)]
  public Vector2 Position = Position;

  [VertexDescriptor(2, VertexType.Float)]
  public Vector2 UV = UV;
}

/// <summary>
/// A common 3d vertex type for primitive shapes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Vertex3(Vector3 Position, ColorF Color, Vector2 UV)
{
  [VertexDescriptor(4, VertexType.Float)]
  public ColorF Color = Color;

  [VertexDescriptor(3, VertexType.Float)]
  public Vector3 Position = Position;

  [VertexDescriptor(2, VertexType.Float)]
  public Vector2 UV = UV;
}

/// <summary>
/// Primitive types supported by <see cref="VertexDescriptor" />s.
/// </summary>
public enum VertexType
{
  UnsignedByte,
  Short,
  UnsignedShort,
  Int,
  UnsignedInt,
  Float,
  Double
}

/// <summary>
/// Associates a <see cref="VertexDescriptor" /> with a vertex field.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field)]
public sealed class VertexDescriptorAttribute : Attribute
{
  public VertexDescriptorAttribute(int count, VertexType type)
  {
    Count = count;
    Type = type;
  }

  public string? Alias { get; set; }
  public int Count { get; set; }
  public VertexType Type { get; set; }

  /// <summary>
  /// True if the resultant components should be normalised to (0, 1) before submission to the GPU.
  /// </summary>
  public bool ShouldNormalize { get; set; }
}

/// <summary>
/// Describes a single vertex.
/// </summary>
[DebuggerDisplay("{Name}: {Count}")]
public readonly record struct VertexDescriptor(string Name, int Offset, int Count, VertexType Type, bool ShouldNormalize)
{
  public int Stride => Count * DetermineSize(Type);

  private static int DetermineSize(VertexType type)
  {
    return type switch
    {
      VertexType.UnsignedByte => sizeof(byte),
      VertexType.Short => sizeof(short),
      VertexType.UnsignedShort => sizeof(ushort),
      VertexType.Int => sizeof(int),
      VertexType.UnsignedInt => sizeof(uint),
      VertexType.Float => sizeof(float),
      VertexType.Double => sizeof(double),

      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
  }
}

/// <summary>
/// Describes a set of <see cref="VertexDescriptor" />s.
/// </summary>
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
        name,
        stride,
        attribute.Count,
        attribute.Type,
        attribute.ShouldNormalize
      );

      stride += descriptor.Stride;

      builder.Add(descriptor);
    }

    return new VertexDescriptorSet(builder.ToImmutable(), stride);
  }
}
