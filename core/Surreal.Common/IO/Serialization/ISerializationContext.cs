namespace Surreal.IO.Serialization;

/// <summary>Context for serialization operations.</summary>
public interface ISerializationContext
{
  ValueTask    SerializeAsync<T>(T value, IBinaryWriter writer, CancellationToken cancellationToken = default);
  ValueTask<T> DeserializeAsync<T>(IBinaryReader reader, CancellationToken cancellationToken = default);
}
