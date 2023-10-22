using LightInject;
using Surreal.Diagnostics.Logging;

namespace Surreal.Utilities;

/// <summary>
/// Abstracts over a service registry, allowing different IoC container depending on game host.
/// </summary>
public interface IServiceRegistry : IServiceProvider
{
  /// <summary>
  /// Registers a service.
  /// </summary>
  void AddService(Type serviceType, Type implementationType);

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
  private static readonly ILog Log = LogFactory.GetLog<ServiceRegistry>();

  private readonly ServiceContainer _container = new();

  public object? GetService(Type serviceType)
  {
    return _container.GetInstance(serviceType);
  }

  public void AddService(Type serviceType, Type implementationType)
  {
    if (implementationType != serviceType)
    {
      Log.Trace($"Registering service {serviceType} with implementation {implementationType}");
    }
    else
    {
      Log.Trace($"Registering service {serviceType}");
    }

    _container.RegisterSingleton(serviceType, implementationType, GenerateName());
  }

  public void AddService(Type serviceType, object instance)
  {
    Log.Trace($"Registering service {serviceType} with instance {instance}");

    _container.RegisterInstance(serviceType, instance, GenerateName());
  }

  private static string GenerateName()
  {
    return Guid.NewGuid().ToString();
  }

  public void Dispose()
  {
    _container.Dispose();
  }
}
