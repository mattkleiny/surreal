namespace Surreal.Serialization;

/// <summary>Context for serialization operations.</summary>
public interface ISerializationContext
{
  ValueTask    SerializeAsync<T>(T value, ISerializationWriter writer, CancellationToken cancellationToken = default);
  ValueTask<T> DeserializeAsync<T>(ISerializationReader reader, CancellationToken cancellationToken = default);
}

/// <summary>A default <see cref="ISerializationContext"/> implementation that uses the <see cref="Serializer{T}"/> metadata.</summary>
public sealed class SerializationContext : ISerializationContext
{
  public ValueTask SerializeAsync<T>(T value, ISerializationWriter writer, CancellationToken cancellationToken = default)
  {
    if (!SerializerRegistry.Instance.TryGetSerializer<T>(out var serializer))
    {
      throw new InvalidOperationException($"Unable to serialize {typeof(T)}, no serializer is registered for it");
    }

    return serializer.SerializeAsync(value, writer, this, cancellationToken);
  }

  public ValueTask<T> DeserializeAsync<T>(ISerializationReader reader, CancellationToken cancellationToken = default)
  {
    if (!SerializerRegistry.Instance.TryGetSerializer<T>(out var serializer))
    {
      throw new InvalidOperationException($"Unable to serialize {typeof(T)}, no serializer is registered for it");
    }

    return serializer.DeserializeAsync(reader, this, cancellationToken);
  }
}
