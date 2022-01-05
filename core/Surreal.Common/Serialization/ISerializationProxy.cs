namespace Surreal.Serialization;

/// <summary>A proxy for reading/writing <see cref="T"/>s from a serialization stream.</summary>
public interface ISerializationProxy<T>
{
  void Serialize(T value, ISerializationWriter writer, SerializationContext context);
  T    Deserialize(ISerializationReader reader, SerializationContext context);
}
