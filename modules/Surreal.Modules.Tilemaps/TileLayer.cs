using System;

namespace Surreal.Modules.Tilemaps {
  public readonly struct TileLayer : IEquatable<TileLayer> {
    public static TileLayer Background { get; } = new(nameof(Background), order: 0);
    public static TileLayer Decoration { get; } = new(nameof(Decoration), order: 0);
    
    public string Name  { get; }
    public int    Order { get; }

    public TileLayer(string name, int order) {
      Name  = name;
      Order = order;
    }

    public          bool Equals(TileLayer other) => string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj)     => obj is TileLayer other && Equals(other);

    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public static bool operator ==(TileLayer left, TileLayer right) => left.Equals(right);
    public static bool operator !=(TileLayer left, TileLayer right) => !left.Equals(right);
  }
}