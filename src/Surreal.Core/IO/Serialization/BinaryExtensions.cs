using System.IO;
using System.Numerics;
using Surreal.Mathematics;

namespace Surreal.IO.Serialization {
  public static class BinaryExtensions {
    public static Seed ReadSeed(this BinaryReader reader) {
      return new Seed(reader.ReadInt32());
    }

    public static Vector2 ReadVector2(this BinaryReader reader) {
      return new Vector2(
          reader.ReadSingle(),
          reader.ReadSingle()
      );
    }

    public static void Write(this BinaryWriter writer, Seed seed) {
      writer.Write(seed.Value);
    }

    public static void Write(this BinaryWriter writer, Vector2 vector) {
      writer.Write(vector.X);
      writer.Write(vector.Y);
    }

    public static T ReadBinaryObject<T>(this BinaryReader reader)
        where T : IBinarySerializable, new() {
      var instance = new T();
      instance.Load(reader);
      return instance;
    }

    public static void WriteBinaryObject<T>(this BinaryWriter writer, T instance)
        where T : IBinarySerializable {
      instance.Save(writer);
    }
  }
}