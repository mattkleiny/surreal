using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Surreal.Data {
  public struct Identifier : IEquatable<Identifier>, IBinarySerializable {
    public static Identifier None       => default;
    public static Identifier Randomized => Guid.NewGuid();

    private string raw;
    private Guid   guid;

    public Identifier(Guid guid) {
      this.guid = guid;
      raw       = guid.ToString();
    }

    public bool   IsValid => !string.IsNullOrEmpty(raw);
    public string Value   => raw;

    public override string ToString() {
      if (string.IsNullOrEmpty(raw)) {
        return "Invalid identifier";
      }

      return raw;
    }

    public          bool Equals(Identifier other) => guid == other.guid;
    public override bool Equals(object? obj)      => obj is Identifier other && Equals(other);

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode() => guid.GetHashCode();

    public static bool operator ==(Identifier left, Identifier right) => left.Equals(right);
    public static bool operator !=(Identifier left, Identifier right) => !left.Equals(right);

    public static implicit operator Identifier(Guid guid) => new(guid);

    void IBinarySerializable.Save(BinaryWriter writer) {
      writer.Write(guid.ToByteArray());
    }

    void IBinarySerializable.Load(BinaryReader reader) {
      raw = new Guid(reader.ReadBytes(16)).ToString();
    }
  }
}