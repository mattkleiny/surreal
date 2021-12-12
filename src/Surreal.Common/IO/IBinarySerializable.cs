namespace Surreal.IO;

/// <summary>Represents a type that can be serialized/deserialized in binary form.</summary>
public interface IBinarySerializable
{
  void Serialize(BinaryWriter writer);
  void Deserialize(BinaryReader reader);
}
