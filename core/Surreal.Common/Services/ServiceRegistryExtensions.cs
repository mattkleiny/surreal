namespace Surreal.Services;

/// <summary>
/// Static extension methods for <see cref="IServiceProvider" /> and related.
/// </summary>
public static class ServiceRegistryExtensions
{
  public static T? GetService<T>(this IServiceProvider provider)
  {
    return (T?)provider.GetService(typeof(T));
  }

  public static IEnumerable<T> GetServices<T>(this IServiceProvider provider)
  {
    if (!provider.TryGetService<IEnumerable<T>>(out var results))
    {
      return Enumerable.Empty<T>();
    }

    return results;
  }

  public static T GetRequiredService<T>(this IServiceProvider provider)
  {
    if (!provider.TryGetService(out T service))
    {
      throw new ServiceNotFoundException($"Unable to locate service {typeof(T).Name}");
    }

    return service;
  }

  public static bool TryGetService<T>(this IServiceProvider provider, out T result)
  {
    var service = provider.GetService(typeof(T));

    if (service != null)
    {
      result = (T)service;
      return true;
    }

    result = default!;
    return false;
  }
}
