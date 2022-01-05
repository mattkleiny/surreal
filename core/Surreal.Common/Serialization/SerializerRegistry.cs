using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Surreal.Serialization;

#pragma warning disable CA2255

/// <summary>A registry for <see cref="ISerializer"/>s.</summary>
public interface ISerializerRegistry
{
  void RegisterSerializer(Type type, ISerializer serializer);
  void RegisterSerializers(AppDomain domain);
  void RegisterSerializers(Assembly assembly);
}

/// <summary>The default <see cref="IServiceRegistry"/> implementation.</summary>
internal sealed class SerializerRegistry : ISerializerRegistry
{
  public static SerializerRegistry Instance { get; } = new();

  private readonly Dictionary<Type, ISerializer> serializersByType = new();

  [ModuleInitializer]
  internal static void Initialize()
  {
    Instance.RegisterSerializers(AppDomain.CurrentDomain);
  }

  public void RegisterSerializer(Type type, ISerializer serializer)
  {
    serializersByType[type] = serializer;
  }

  public void RegisterSerializers(AppDomain domain)
  {
    foreach (var assembly in domain.GetAssemblies())
    {
      RegisterSerializers(assembly);
    }
  }

  public void RegisterSerializers(Assembly assembly)
  {
    var candidates =
      from type in assembly.GetTypes()
      from attribute in type.GetCustomAttributes<SerializerAttribute>(inherit: true)
      select new { Attribute = attribute, Type = type };

    foreach (var candidate in candidates)
    {
      candidate.Attribute.RegisterSerializer(candidate.Type, this);
    }
  }

  public bool TryGetSerializer<T>([MaybeNullWhen(false)] out Serializer<T> result)
  {
    if (serializersByType.TryGetValue(typeof(T), out var serializer))
    {
      result = (Serializer<T>) serializer;
      return true;
    }

    result = default;
    return false;
  }
}
