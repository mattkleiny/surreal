namespace Surreal.Utilities;

/// <summary>
/// Abstracts over a service registry, allowing different IoC container depending on game host.
/// </summary>
public interface IServiceRegistry : IServiceProvider
{
  /// <summary>
  /// Registers a service.
  /// </summary>
  void AddService(Type serviceType, object instance);
}

/// <summary>
/// A simple default <see cref="IServiceRegistry" /> implementation.
/// </summary>
public sealed class ServiceRegistry : IServiceRegistry, IDisposable
{
  private readonly ConcurrentDictionary<Type, object> _instances = new();

  public object? GetService(Type serviceType)
  {
    _instances.TryGetValue(serviceType, out var instance);

    return instance;
  }

  public void AddService(Type serviceType, object instance)
  {
    _instances.TryAdd(serviceType, instance);
  }

  public void Dispose()
  {
    foreach (var instance in _instances.Values)
    {
      if (instance is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    _instances.Clear();
  }
}
