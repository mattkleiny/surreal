using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Surreal.Serialization;

#pragma warning disable CA2255

/// <summary>Abstracts over all possible <see cref="BinarySerializer{T}"/> types.</summary>
public abstract class BinarySerializer
{
  private static ISerializationContext Context { get; } = new SerializationContext();

  public static ValueTask SerializeAsync<T>(T value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    return Context.SerializeAsync(value, writer, cancellationToken);
  }

  public static ValueTask<T> DeserializeAsync<T>(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    return Context.DeserializeAsync<T>(reader, cancellationToken);
  }

  /// <summary>A default <see cref="ISerializationContext"/> implementation that uses the <see cref="BinarySerializer{T}"/> metadata.</summary>
  internal sealed class SerializationContext : ISerializationContext
  {
    private readonly Dictionary<Type, BinarySerializer> serializersByType = new();

    [ModuleInitializer]
    internal static void Initialize()
    {
      SerializationContext context = ((SerializationContext) Context);

      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        context.RegisterSerializers(assembly);
      }
    }

    private void RegisterSerializers(Assembly assembly)
    {
      var candidates =
        from type in assembly.GetTypes()
        from attribute in type.GetCustomAttributes<SerializerAttribute>(inherit: true)
        select new { Attribute = attribute, Type = type };

      foreach (var candidate in candidates)
      {
        var serializer = (BinarySerializer) Activator.CreateInstance(candidate.Type)!;

        RegisterSerializer(candidate.Attribute.Type, serializer);
      }
    }

    private void RegisterSerializer(Type type, BinarySerializer serializer)
    {
      serializersByType[type] = serializer;
    }

    private bool TryGetSerializer<T>([MaybeNullWhen(false)] out BinarySerializer<T> result)
    {
      if (serializersByType.TryGetValue(typeof(T), out var serializer))
      {
        result = (BinarySerializer<T>) serializer;
        return true;
      }

      result = default;
      return false;
    }

    ValueTask ISerializationContext.SerializeAsync<T>(T value, IBinaryWriter writer, CancellationToken cancellationToken)
    {
      if (!TryGetSerializer<T>(out var serializer))
      {
        throw new InvalidOperationException($"Unable to serialize {typeof(T)}, no serializer is registered for it");
      }

      return serializer.SerializeAsync(value, writer, this, cancellationToken);
    }

    ValueTask<T> ISerializationContext.DeserializeAsync<T>(IBinaryReader reader, CancellationToken cancellationToken)
    {
      if (!TryGetSerializer<T>(out var serializer))
      {
        throw new InvalidOperationException($"Unable to serialize {typeof(T)}, no serializer is registered for it");
      }

      return serializer.DeserializeAsync(reader, this, cancellationToken);
    }
  }
}

/// <summary>A serializer for reading/writing <see cref="T"/>s.</summary>
public abstract class BinarySerializer<T> : BinarySerializer
{
  public abstract ValueTask    SerializeAsync(T value, IBinaryWriter writer, ISerializationContext context, CancellationToken cancellationToken = default);
  public abstract ValueTask<T> DeserializeAsync(IBinaryReader reader, ISerializationContext context, CancellationToken cancellationToken = default);
}

/// <summary>Associates a <see cref="BinarySerializer{T}"/> with it's source type.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public sealed class SerializerAttribute : Attribute
{
  public SerializerAttribute(Type type)
  {
    Type = type;
  }

  public Type Type { get; }
}
