using System;

namespace Surreal.Graphics.Materials
{
  public readonly struct MaterialProperty<T> : IEquatable<MaterialProperty<T>>
  {
    public string Name { get; }
    public int    Hash { get; }

    public MaterialProperty(string name)
    {
      Name = name;
      Hash = name.GetHashCode();
    }

    public override string ToString() => Name;

    public          bool Equals(MaterialProperty<T> other) => Hash == other.Hash;
    public override bool Equals(object? obj)               => obj is MaterialProperty<T> other && Equals(other);

    public override int GetHashCode() => Hash;

    public static bool operator ==(MaterialProperty<T> left, MaterialProperty<T> right) => left.Equals(right);
    public static bool operator !=(MaterialProperty<T> left, MaterialProperty<T> right) => !left.Equals(right);
  }
}