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

/// <summary>Abstracts over all possible <see cref="BinarySerializer{T}"/> types.</summary>
public abstract class BinarySerializer
{
  private static readonly Dictionary<Type, BinarySerializer> SerializersByType = new();

  [ModuleInitializer]
  internal static void Initialize()
  {
    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      var candidates =
        from type in assembly.GetTypes()
        from attribute in type.GetCustomAttributes<BinarySerializerAttribute>(inherit: true)
        select new { Attribute = attribute, Type = type };

      foreach (var candidate in candidates)
      {
        var serializer = (BinarySerializer) Activator.CreateInstance(candidate.Type)!;

        SerializersByType[candidate.Attribute.Type] = serializer;
      }
    }
  }

  private static bool TryGetSerializer<T>([MaybeNullWhen(false)] out BinarySerializer<T> result)
  {
    if (SerializersByType.TryGetValue(typeof(T), out var serializer))
    {
      result = (BinarySerializer<T>) serializer;
      return true;
    }

    result = default;
    return false;
  }

  public static ValueTask SerializeAsync<T>(T value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    if (!TryGetSerializer<T>(out var serializer))
    {
      throw new InvalidOperationException($"Unable to serialize {typeof(T)}, no serializer is registered for it");
    }

    return serializer.SerializeAsync(value, writer, cancellationToken);
  }

  public static ValueTask<T> DeserializeAsync<T>(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    if (!TryGetSerializer<T>(out var serializer))
    {
      throw new InvalidOperationException($"Unable to deserialize {typeof(T)}, no serializer is registered for it");
    }

    return serializer.DeserializeAsync(reader, cancellationToken);
  }
}

/// <summary>A serializer for reading/writing <see cref="T"/>s.</summary>
public abstract class BinarySerializer<T> : BinarySerializer
{
  public abstract ValueTask SerializeAsync(T value, IBinaryWriter writer, CancellationToken cancellationToken = default);
  public abstract ValueTask<T> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default);
}
