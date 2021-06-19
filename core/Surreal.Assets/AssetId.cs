using System;
using Surreal.IO;

namespace Surreal.Assets
{
  public readonly struct AssetId : IEquatable<AssetId>
  {
    private readonly int hash;

    public AssetId(Type type, Path path)
    {
      hash = $"{type.Name}{path}".GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    public          bool Equals(AssetId other) => hash == other.hash;
    public override bool Equals(object? obj)   => obj is AssetId other && Equals(other);

    public override int GetHashCode() => hash;

    public static bool operator ==(AssetId left, AssetId right) => left.Equals(right);
    public static bool operator !=(AssetId left, AssetId right) => !left.Equals(right);
  }
}