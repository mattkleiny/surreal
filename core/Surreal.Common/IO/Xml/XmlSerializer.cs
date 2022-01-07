using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Surreal.IO.Xml;

// ReSharper disable MemberHidesStaticFromOuterClass
#pragma warning disable CA2255

/// <summary>Associates a <see cref="XmlSerializer{T}"/> with it's source type.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public sealed class XmlSerializerAttribute : Attribute
{
  public XmlSerializerAttribute(Type type)
  {
    Type = type;
  }

  public Type Type { get; }
}

/// <summary>Context for xml serialization operations.</summary>
public interface IXmlSerializationContext
{
  ValueTask<object> DeserializeAsync(Type type, XElement element, CancellationToken cancellationToken = default);
  ValueTask<T>      DeserializeAsync<T>(XElement element, CancellationToken cancellationToken = default) where T : notnull;
}

/// <summary>Represents all possible <see cref="XmlSerializer{T}"/>s.</summary>
public interface IXmlSerializer
{
  ValueTask<object> DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken = default);
}

/// <summary>Parses <see cref="T"/>s from <see cref="XElement"/>s.</summary>
public abstract class XmlSerializer<T> : IXmlSerializer
  where T : notnull
{
  public abstract ValueTask<T> DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken = default);

  async ValueTask<object> IXmlSerializer.DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken)
  {
    return await DeserializeAsync(element, context, cancellationToken);
  }
}

/// <summary>Abstracts over all possible <see cref="XmlSerializer{T}"/> types.</summary>
public static class XmlSerializer
{
  private static SerializationContext Context { get; } = new SerializationContext();

  public static ValueTask<object> DeserializeAsync(Type type, XElement element, CancellationToken cancellationToken = default)
  {
    return Context.DeserializeAsync(type, element, cancellationToken);
  }

  public static ValueTask<T> DeserializeAsync<T>(XElement element, CancellationToken cancellationToken = default)
    where T : notnull
  {
    return Context.DeserializeAsync<T>(element, cancellationToken);
  }

  /// <summary>A default <see cref="IXmlSerializationContext"/> implementation that uses the <see cref="XmlSerializer{T}"/> metadata.</summary>
  internal sealed class SerializationContext : IXmlSerializationContext
  {
    private readonly Dictionary<Type, IXmlSerializer> serializersByType = new();

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
        from attribute in type.GetCustomAttributes<XmlSerializerAttribute>(inherit: true)
        select new { Attribute = attribute, Type = type };

      foreach (var candidate in candidates)
      {
        var serializer = (IXmlSerializer) Activator.CreateInstance(candidate.Type)!;

        RegisterSerializer(candidate.Attribute.Type, serializer);
      }
    }

    private void RegisterSerializer(Type type, IXmlSerializer serializer)
    {
      serializersByType[type] = serializer;
    }

    private bool TryGetSerializer(Type type, [MaybeNullWhen(false)] out IXmlSerializer result)
    {
      return serializersByType.TryGetValue(type, out result);
    }

    public ValueTask<object> DeserializeAsync(Type type, XElement element, CancellationToken cancellationToken = default)
    {
      if (!TryGetSerializer(type, out var serializer))
      {
        throw new InvalidOperationException($"Unable to deserialize {type}, no serializer is registered for it");
      }

      return serializer.DeserializeAsync(element, this, cancellationToken);
    }

    public ValueTask<T> DeserializeAsync<T>(XElement element, CancellationToken cancellationToken = default)
      where T : notnull
    {
      if (!TryGetSerializer(typeof(T), out var serializer))
      {
        throw new InvalidOperationException($"Unable to deserialize {typeof(T)}, no serializer is registered for it");
      }

      return ((XmlSerializer<T>) serializer).DeserializeAsync(element, this, cancellationToken);
    }
  }
}
