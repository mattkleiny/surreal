using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Surreal.IO.Binary;

// ReSharper disable MemberHidesStaticFromOuterClass
#pragma warning disable CA2255

/// <summary>Associates a <see cref="BinarySerializer{T}"/> with it's source type.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public sealed class BinarySerializerAttribute : Attribute
{
  public BinarySerializerAttribute(Type type)
  {
    Type = type;
  }

  public Type Type { get; }
}

/// <summary>Context for binary serialization operations.</summary>
public interface IBinarySerializationContext
{
  ValueTask    SerializeAsync<T>(T value, IBinaryWriter writer, CancellationToken cancellationToken = default);
  ValueTask<T> DeserializeAsync<T>(IBinaryReader reader, CancellationToken cancellationToken = default);
}

/// <summary>Represents all possible <see cref="BinarySerializer{T}"/>s.</summary>
public interface IBinarySerializer
{
}

/// <summary>A serializer for reading/writing <see cref="T"/>s.</summary>
public abstract class BinarySerializer<T> : IBinarySerializer
{
  public abstract ValueTask    SerializeAsync(T value, IBinaryWriter writer, IBinarySerializationContext context, CancellationToken cancellationToken = default);
  public abstract ValueTask<T> DeserializeAsync(IBinaryReader reader, IBinarySerializationContext context, CancellationToken cancellationToken = default);
}

/// <summary>Abstracts over all possible <see cref="BinarySerializer{T}"/> types.</summary>
public static class BinarySerializer
{
  private static SerializationContext Context { get; } = new SerializationContext();

  public static ValueTask SerializeAsync<T>(T value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    return Context.SerializeAsync(value, writer, cancellationToken);
  }

  public static ValueTask<T> DeserializeAsync<T>(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    return Context.DeserializeAsync<T>(reader, cancellationToken);
  }

  /// <summary>A default <see cref="IBinarySerializationContext"/> implementation that uses the <see cref="BinarySerializer{T}"/> metadata.</summary>
  internal sealed class SerializationContext : IBinarySerializationContext
  {
    private readonly Dictionary<Type, IBinarySerializer> serializersByType = new();

    [ModuleInitializer]
    internal static void Initialize()
    {
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        Context.RegisterSerializers(assembly);
      }
    }

    private void RegisterSerializers(Assembly assembly)
    {
      var candidates =
        from type in assembly.GetTypes()
        from attribute in type.GetCustomAttributes<BinarySerializerAttribute>(inherit: true)
        select new { Attribute = attribute, Type = type };

      foreach (var candidate in candidates)
      {
        var serializer = (IBinarySerializer) Activator.CreateInstance(candidate.Type)!;

        RegisterSerializer(candidate.Attribute.Type, serializer);
      }
    }

    private void RegisterSerializer(Type type, IBinarySerializer serializer)
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

    public ValueTask SerializeAsync<T>(T value, IBinaryWriter writer, CancellationToken cancellationToken)
    {
      if (!TryGetSerializer<T>(out var serializer))
      {
        throw new InvalidOperationException($"Unable to serialize {typeof(T)}, no serializer is registered for it");
      }

      return serializer.SerializeAsync(value, writer, this, cancellationToken);
    }

    public ValueTask<T> DeserializeAsync<T>(IBinaryReader reader, CancellationToken cancellationToken)
    {
      if (!TryGetSerializer<T>(out var serializer))
      {
        throw new InvalidOperationException($"Unable to serialize {typeof(T)}, no serializer is registered for it");
      }

      return serializer.DeserializeAsync(reader, this, cancellationToken);
    }
  }
}
