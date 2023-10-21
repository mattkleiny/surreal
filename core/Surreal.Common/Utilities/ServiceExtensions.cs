namespace Surreal.Utilities;

/// <summary>
/// Indicates a service is not available.
/// </summary>
public class ServiceNotFoundException(string message) : ApplicationException(message);

/// <summary>
/// Indicates that the class should be registered as a service.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public class RegisterServiceAttribute(Type? serviceType = null) : Attribute
{
  /// <summary>
  /// The type of the service to register.
  /// </summary>
  public Type? ServiceType { get; } = serviceType;

  /// <summary>
  /// Registers the service in the given <see cref="IServiceRegistry"/>.
  /// </summary>
  public virtual void RegisterService(IServiceRegistry registry, Type implementationType)
  {
    var instance = Activator.CreateInstance(implementationType);

    if (instance == null)
    {
      throw new InvalidOperationException($"Unable to active service of type {implementationType}");
    }

    registry.AddService(ServiceType ?? implementationType, instance);
  }
}

/// <summary>
/// Indicates the associated property should be injected with a service.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
public sealed class InjectAttribute : Attribute;

/// <summary>
/// Indicates a module that can be registered in a <see cref="IServiceRegistry"/>.
/// </summary>
public interface IServiceModule
{
  /// <summary>
  /// Registers all services in the module.
  /// </summary>
  void RegisterServices(IServiceRegistry registry);
}

/// <summary>
/// Helpers for working with <see cref="IServiceProvider"/> and <see cref="IServiceRegistry"/>.
/// </summary>
public static class ServiceExtensions
{
  /// <summary>
  /// Adds a service to the registry.
  /// </summary>
  public static void AddService<T>(this IServiceRegistry registry, T service)
    where T : class
  {
    registry.AddService(typeof(T), service);
  }

  /// <summary>
  /// Registers a <see cref="IServiceModule"/>.
  /// </summary>
  public static void AddModule(this IServiceRegistry registry, IServiceModule module)
  {
    module.RegisterServices(registry);
  }

  /// <summary>
  /// Registers all services marked with <see cref="RegisterServiceAttribute"/> in the given assembly.
  /// </summary>
  public static void AddAssemblyServices(this IServiceRegistry registry, Assembly assembly)
  {
    foreach (var type in assembly.GetTypes())
    {
      if (type.TryGetCustomAttribute(out RegisterServiceAttribute attribute, inherit: true))
      {
        attribute.RegisterService(registry, type);
      }
    }
  }

  /// <summary>
  /// Gets the service of type <typeparamref name="T"/>. If the service is not found, returns <c>null</c>.
  /// </summary>
  public static T? GetService<T>(this IServiceProvider provider)
    where T : class
  {
    return (T?)provider.GetService(typeof(T));
  }

  /// <summary>
  /// Gets all services of type <typeparamref name="T"/>.
  /// </summary>
  public static IEnumerable<T> GetServices<T>(this IServiceProvider provider)
    where T : class
  {
    return GetService<IEnumerable<T>>(provider) ?? Enumerable.Empty<T>();
  }

  /// <summary>
  /// Attempts to get the service of type <typeparamref name="T"/>. If the service is not found, returns <c>false</c>.
  /// </summary>
  public static bool TryGetService<T>(this IServiceProvider provider, [NotNullWhen(true)] out T result)
    where T : class
  {
    result = provider.GetService<T>()!;

    return result != null!;
  }

  /// <summary>
  /// Gets the service of type <typeparamref name="T"/>. If the service is not found, throws an exception.
  /// </summary>
  public static T GetServiceOrThrow<T>(this IServiceProvider provider)
    where T : class
  {
    if (!provider.TryGetService(out T result))
    {
      throw new ServiceNotFoundException($"Unable to locate service of type {typeof(T)}");
    }

    return result;
  }

  /// <summary>
  /// Gets the service of type <typeparamref name="T"/>. If the service is not found, throws an exception.
  /// </summary>
  public static T GetServiceOrDefault<T>(this IServiceProvider provider, T defaultValue = default!)
    where T : class
  {
    if (!provider.TryGetService(out T result))
    {
      return defaultValue;
    }

    return result;
  }

  /// <summary>
  /// Injects <see cref="InjectAttribute"/>-annotated properties on the given target.
  /// </summary>
  public static void Inject(this IServiceProvider provider, object target)
  {
    const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    static void InjectFields(IServiceProvider provider, object target)
    {
      var fields =
        from field in target.GetType().GetFields(bindingFlags)
        where field.GetCustomAttribute<InjectAttribute>() != null
        select field;

      foreach (var field in fields)
      {
        var service = provider.GetService(field.FieldType);
        if (service != null)
        {
          field.SetValue(target, service);
        }
      }
    }


    static void InjectProperties(IServiceProvider provider, object target)
    {
      var properties =
        from property in target.GetType().GetProperties(bindingFlags)
        where property.GetCustomAttribute<InjectAttribute>() != null
        select property;

      foreach (var property in properties)
      {
        var service = provider.GetService(property.PropertyType);
        if (service != null)
        {
          property.SetValue(target, service);
        }
      }
    }

    static void InjectMethods(IServiceProvider provider, object target)
    {
      var methods =
        from method in target.GetType().GetMethods(bindingFlags)
        where method.GetCustomAttribute<InjectAttribute>() != null
        select method;

      foreach (var method in methods)
      {
        var parameters = method.GetParameters();
        var arguments = new object?[parameters.Length];

        for (var i = 0; i < arguments.Length; i++)
        {
          arguments[i] = provider.GetService(parameters[i].ParameterType);
        }

        method.Invoke(target, arguments);
      }
    }

    InjectFields(provider, target);
    InjectProperties(provider, target);
    InjectMethods(provider, target);
  }
}
