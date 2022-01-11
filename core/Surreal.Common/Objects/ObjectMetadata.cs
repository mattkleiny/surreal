using System.Reflection;
using JetBrains.Annotations;

namespace Surreal.Objects;

/// <summary>Binds the given event for use in metadata and reflection.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property)]
public sealed class BindAttribute : Attribute
{
}

/// <summary>Base class for all <see cref="ObjectMetadata{T}"/> types.</summary>
public abstract record ObjectMetadata
{
  private static readonly ConcurrentDictionary<Type, ObjectMetadata> MetadataByType = new();

  /// <summary>Creates <see cref="ObjectMetadata{T}"/> metadata for the given instance.</summary>
  public static ObjectMetadata<T> Create<T>()
    where T : notnull
  {
    var type = typeof(T);

    if (!MetadataByType.TryGetValue(type, out var metadata))
    {
      MetadataByType[type] = metadata = new ObjectMetadata<T>();
    }

    return (ObjectMetadata<T>) metadata;
  }
}

/// <summary>An object that can be introspected for editing/etc</summary>
public sealed record ObjectMetadata<T> : ObjectMetadata
{
  private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

  public ImmutableList<EventMetadata>    Events     { get; } = EventMetadata.DiscoverAll(typeof(T));
  public ImmutableList<MethodMetadata>   Methods    { get; } = MethodMetadata.DiscoverAll(typeof(T));
  public ImmutableList<PropertyMetadata> Properties { get; } = PropertyMetadata.DiscoverAll(typeof(T));

  /// <summary>Contains metadata about a bound event.</summary>
  public sealed class EventMetadata
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

    private readonly EventInfo eventInfo;

    public EventMetadata(EventInfo eventInfo)
    {
      this.eventInfo = eventInfo;
    }

    public string Name => eventInfo.Name;

    public void Invoke(T instance)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>Contains metadata about a bound method.</summary>
  public sealed class MethodMetadata
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

    private readonly MethodInfo methodInfo;

    public MethodMetadata(MethodInfo methodInfo)
    {
      this.methodInfo = methodInfo;
    }

    public string Name => methodInfo.Name;

    public object? Invoke(T instance, params object?[] arguments)
    {
      return methodInfo.Invoke(instance, arguments);
    }
  }

  /// <summary>Contains metadata about a bound property.</summary>
  public sealed class PropertyMetadata
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

    private readonly PropertyInfo propertyInfo;

    public PropertyMetadata(PropertyInfo propertyInfo)
    {
      this.propertyInfo = propertyInfo;
    }

    public string Name => propertyInfo.Name;

    public object? GetValue(object instance)
    {
      return propertyInfo.GetValue(instance);
    }

    public void SetValue(T instance, object? value)
    {
      propertyInfo.SetValue(instance, value);
    }
  }
}
