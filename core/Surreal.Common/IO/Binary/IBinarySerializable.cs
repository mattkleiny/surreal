namespace Surreal.IO.Binary;

/// <summary>A type that can be serialized/deserialized directly to/from a binary stream.</summary>
public interface IBinarySerializable<T>
{
  /// <summary>Deserializes the type from the given reader.</summary>
  static abstract ValueTask<T> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default);

  /// <summary>Serializes the type to the given writer.</summary>
  ValueTask SerializeAsync(IBinaryWriter writer, CancellationToken cancellationToken = default);
}
