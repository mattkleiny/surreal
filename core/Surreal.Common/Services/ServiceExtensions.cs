namespace Surreal.Services;

/// <summary>
/// Indicates a service is not available.
/// </summary>
public class ServiceNotFoundException(string message) : ApplicationException(message);

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
    if (provider is IServiceRegistry registry)
    {
      return registry.GetServices(typeof(T)).Cast<T>();
    }

    return GetService<IEnumerable<T>>(provider)?.ToArray() ?? Enumerable.Empty<T>();
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
  /// Instantiates the given type using the container.
  /// </summary>
  public static T Instantiate<T>(this IServiceRegistry registry)
  {
    return (T)registry.Instantiate(typeof(T));
  }

  /// <summary>
  /// Initializes all services that implement <see cref="IInitializable"/>.
  /// </summary>
  public static void Initialize(this IServiceRegistry registry)
  {
    foreach (var service in registry.GetServices<IInitializable>())
    {
      service.Initialize();
    }
  }
}
