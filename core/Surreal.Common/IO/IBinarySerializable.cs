namespace Surreal.IO;

/// <summary>
/// Represents a type that can be serialized to and from binary.
/// </summary>
public interface IBinarySerializable
{
  /// <summary>
  /// Converts the given binary to this type.
  /// </summary>
  static abstract object FromBinary(FastBinaryReader reader);

  /// <summary>
  /// Converts this type to binary.
  /// </summary>
  void ToBinary(FastBinaryWriter writer);
}

/// <summary>
/// Represents a type that can be serialized to and from binary.
/// </summary>
public interface IBinarySerializable<out TSelf> : IBinarySerializable
  where TSelf : IBinarySerializable<TSelf>
{
  /// <summary>
  /// Converts the given binary to this type.
  /// </summary>
  new static abstract TSelf FromBinary(FastBinaryReader reader);

  /// <inheritdoc/>
  static object IBinarySerializable.FromBinary(FastBinaryReader reader) => TSelf.FromBinary(reader);
}

/// <summary>
/// Helpers for working with <see cref="IBinarySerializable"/> types.
/// </summary>
public static class BinarySerializableExtensions
{
  /// <summary>
  /// Converts the given <see cref="IBinarySerializable"/> to a byte array.
  /// </summary>
  public static Memory<byte> ToByteArray<T>(this T serializable)
    where T : IBinarySerializable<T>
  {
    using var stream = new MemoryStream();

    using (var writer = new FastBinaryWriter(stream))
    {
      serializable.ToBinary(writer);
    }

    return stream.ToArray();
  }
}
