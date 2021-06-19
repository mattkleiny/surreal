using System;
using Surreal.IO;

namespace Surreal.Assets
{
  public readonly struct AssetId : IEquatable<AssetId>
  {
    public AssetId(Type type, Path path)
    {
      Type = type;
      Path = path;
      Hash = $"{type.Name}{path.ToString()}".GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    public bool IsValid => Hash != 0;

    public Type Type { get; }
    public Path Path { get; }
    public int  Hash { get; }

    public override string ToString() => $"{Path.ToString()} <{Hash.ToString()}>";

    public          bool Equals(AssetId other) => Hash == other.Hash;
    public override bool Equals(object? obj)   => obj is AssetId other && Equals(other);

    public override int GetHashCode() => Hash;

    public static bool operator ==(AssetId left, AssetId right) => left.Equals(right);
    public static bool operator !=(AssetId left, AssetId right) => !left.Equals(right);
  }
}