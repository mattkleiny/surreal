using JetBrains.Annotations;

namespace Surreal.Serialization;

/// <summary>Associates a <see cref="Serializer{T}"/> with it's source type.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public sealed class SerializerAttribute : Attribute
{
  public SerializerAttribute(Type type)
  {
    Type = type;
  }

  public Type Type { get; }

  public void RegisterSerializer(Type serializerType, ISerializerRegistry registry)
  {
    registry.RegisterSerializer(Type, (ISerializer) Activator.CreateInstance(serializerType)!);
  }
}

/// <summary>Abstracts over all possible <see cref="Serializer{T}"/> types.</summary>
public interface ISerializer
{
}

/// <summary>A serializer for reading/writing <see cref="T"/>s.</summary>
public abstract class Serializer<T> : ISerializer
{
  public abstract ValueTask    SerializeAsync(T value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default);
  public abstract ValueTask<T> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default);
}
