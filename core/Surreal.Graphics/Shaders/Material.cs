namespace Surreal.Graphics.Shaders;

/// <summary>A key used to sort <see cref="Material"/>s for efficient batching.</summary>
public readonly record struct MaterialSortingKey(ulong Key) : IComparable<MaterialSortingKey>
{
  public int CompareTo(MaterialSortingKey other) => Key.CompareTo(other.Key);

  public static bool operator <(MaterialSortingKey left, MaterialSortingKey right)  => left.Key < right.Key;
  public static bool operator >(MaterialSortingKey left, MaterialSortingKey right)  => left.Key > right.Key;
  public static bool operator <=(MaterialSortingKey left, MaterialSortingKey right) => left.Key <= right.Key;
  public static bool operator >=(MaterialSortingKey left, MaterialSortingKey right) => left.Key >= right.Key;
}

/// <summary>Describes a single property of a <see cref="Material"/>.</summary>
public readonly record struct MaterialProperty<T>(string Name)
{
  public int Hash { get; } = Name.GetHashCode(StringComparison.Ordinal);

  public override string ToString()    => Name;
  public override int    GetHashCode() => Hash;

  public bool Equals(MaterialProperty<T> other) => Hash == other.Hash;
}

/// <summary>A material manages and batches GPU shader effects.</summary>
public abstract class Material
{
  public abstract MaterialSortingKey SortingKey { get; }
}
