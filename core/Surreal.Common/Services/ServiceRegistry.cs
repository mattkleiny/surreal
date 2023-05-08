namespace Surreal.Services;

/// <summary>
/// A simple default <see cref="IServiceRegistry" /> implementation.
/// </summary>
public sealed class ServiceRegistry : IServiceRegistry
{
  private readonly ConcurrentDictionary<Type, object> _instancesByType = new();

  public object? GetService(Type serviceType)
  {
    if (serviceType == typeof(IServiceRegistry) || serviceType == typeof(ServiceRegistry))
    {
      return this;
    }

    if (_instancesByType.TryGetValue(serviceType, out var instance))
    {
      return instance;
    }

    return null;
  }

  public void RegisterService(Type serviceType, object instance)
  {
    _instancesByType[serviceType] = instance;
  }

  public void Dispose()
  {
    foreach (var instance in _instancesByType.Values)
    {
      if (instance is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }
  }
}
