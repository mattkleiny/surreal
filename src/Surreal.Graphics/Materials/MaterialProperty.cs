using System;

namespace Surreal.Graphics.Materials
{
  public readonly record struct MaterialProperty<T>(string Name)
  {
    public int Hash { get; } = Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public override string ToString()    => Name;
    public override int    GetHashCode() => Hash;

    public bool Equals(MaterialProperty<T> other) => Hash == other.Hash;
  }
}
