namespace Surreal.Graphics.Materials;

/// <summary>A key used to sort <see cref="Material"/>s for efficient batching.</summary>
public readonly record struct MaterialSortingKey(ulong Key) : IComparable<MaterialSortingKey>
{
  public int CompareTo(MaterialSortingKey other) => Key.CompareTo(other.Key);

  public static bool operator <(MaterialSortingKey left, MaterialSortingKey right)  => left.Key < right.Key;
  public static bool operator >(MaterialSortingKey left, MaterialSortingKey right)  => left.Key > right.Key;
  public static bool operator <=(MaterialSortingKey left, MaterialSortingKey right) => left.Key <= right.Key;
  public static bool operator >=(MaterialSortingKey left, MaterialSortingKey right) => left.Key >= right.Key;
}

/// <summary>A material manages and batches GPU shader effects.</summary>
public abstract class Material : GraphicsResource
{
  public abstract MaterialSortingKey SortingKey { get; }
}

/// <summary>A shader-based <see cref="Material"/>.</summary>
public abstract class ShaderMaterial : Material
{
}
