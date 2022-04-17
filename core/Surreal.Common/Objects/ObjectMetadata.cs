using System.Reflection;
using JetBrains.Annotations;

namespace Surreal.Objects;

/// <summary>Binds the given event for use in metadata and reflection.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property)]
public sealed class BindAttribute : Attribute
{
}

/// <summary>An object that can be introspected for editing/etc</summary>
public sealed record ObjectMetadata(Type Type)
{
  private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

  private static readonly ConcurrentDictionary<Type, ObjectMetadata> MetadataByType = new();

  public ImmutableList<EventMetadata>    Events     { get; } = EventMetadata.DiscoverAll(Type);
  public ImmutableList<MethodMetadata>   Methods    { get; } = MethodMetadata.DiscoverAll(Type);
  public ImmutableList<PropertyMetadata> Properties { get; } = PropertyMetadata.DiscoverAll(Type);

  /// <summary>Creates <see cref="ObjectMetadata"/> metadata for the given instance.</summary>
  public static ObjectMetadata Create(Type type)
  {
    if (!MetadataByType.TryGetValue(type, out var metadata))
    {
      MetadataByType[type] = metadata = new ObjectMetadata(type);
    }

    return metadata;
  }

  /// <summary>Contains metadata about a bound event.</summary>
  public sealed record EventMetadata(EventInfo EventInfo)
  {
    public static ImmutableList<EventMetadata> DiscoverAll(Type type)
    {
      var results = ImmutableList.CreateBuilder<EventMetadata>();

      foreach (var eventInfo in type.GetEvents(Flags))
      {
        if (eventInfo.GetCustomAttribute<BindAttribute>() != null)
        {
          results.Add(new EventMetadata(eventInfo));
        }
      }

      return results.ToImmutable();
    }

    public string Name => EventInfo.Name;

    public void Invoke(object instance)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>Contains metadata about a bound method.</summary>
  public sealed record MethodMetadata(MethodInfo MethodInfo)
  {
    public static ImmutableList<MethodMetadata> DiscoverAll(Type type)
    {
      var results = ImmutableList.CreateBuilder<MethodMetadata>();

      foreach (var methodInfo in type.GetMethods(Flags))
      {
        if (methodInfo.GetCustomAttribute<BindAttribute>() != null)
        {
          results.Add(new MethodMetadata(methodInfo));
        }
      }

      return results.ToImmutable();
    }

    public string Name => MethodInfo.Name;

    public object? Invoke(object instance, params object?[] arguments)
    {
      return MethodInfo.Invoke(instance, arguments);
    }
  }

  /// <summary>Contains metadata about a bound property.</summary>
  public sealed record PropertyMetadata(PropertyInfo PropertyInfo)
  {
    public static ImmutableList<PropertyMetadata> DiscoverAll(Type type)
    {
      var results = ImmutableList.CreateBuilder<PropertyMetadata>();

      foreach (var propertyInfo in type.GetProperties(Flags))
      {
        if (propertyInfo.GetCustomAttribute<BindAttribute>() != null)
        {
          results.Add(new PropertyMetadata(propertyInfo));
        }
      }

      return results.ToImmutable();
    }

    public string Name => PropertyInfo.Name;

    public object? GetValue(object instance)
    {
      return PropertyInfo.GetValue(instance);
    }

    public void SetValue(object instance, object? value)
    {
      PropertyInfo.SetValue(instance, value);
    }
  }
}
