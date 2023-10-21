namespace Surreal.IO;

/// <summary>
/// Represents a type that can be serialized to and from a binary stream.
/// </summary>
public interface IBinarySerializable
{
  /// <summary>
  /// Saves this type to the given <paramref name="writer"/>.
  /// </summary>
  void Save(FastBinaryWriter writer);

  /// <summary>
  /// Loads this type from the given <paramref name="reader"/>.
  /// </summary>
  void Load(FastBinaryReader reader);
}
