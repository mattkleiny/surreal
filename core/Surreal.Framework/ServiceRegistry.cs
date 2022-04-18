using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Surreal;

/// <summary>A <see cref="IServiceRegistry"/> that wraps a <see cref="IServiceCollection"/>.</summary>
public sealed class ServiceRegistry : IServiceRegistry
{
  private readonly Dictionary<Type, object> resolutionCache = new();
  private readonly ServiceCollection collection = new();

  private ServiceProvider? provider;
  private bool isSealed;
  private bool isDirty;

  public void RegisterService(ServiceLifetime lifetime, Type serviceType, Type implementationType)
  {
    CheckNotSealed();

    collection.Add(new ServiceDescriptor(serviceType, implementationType, ConvertLifetime(lifetime)));

    isDirty = true;
  }

  public void RegisterService(Type serviceType, object instance)
  {
    CheckNotSealed();

    collection.Add(new ServiceDescriptor(serviceType, instance));

    isDirty = true;
  }

  public void ReplaceService(ServiceLifetime lifetime, Type serviceType, Type implementationType)
  {
    CheckNotSealed();

    collection.RemoveAll(serviceType);
    collection.Add(new ServiceDescriptor(serviceType, implementationType, ConvertLifetime(lifetime)));

    isDirty = true;
  }

  public void ReplaceService(Type serviceType, object instance)
  {
    CheckNotSealed();

    collection.RemoveAll(serviceType);
    collection.Add(new ServiceDescriptor(serviceType, instance));

    isDirty = true;
  }

  public void SealRegistry()
  {
    isSealed = true;
  }

  object? IServiceProvider.GetService(Type serviceType)
  {
    if (isDirty || provider == null)
    {
      provider = collection.BuildServiceProvider();

      resolutionCache.Clear();
      isDirty = false;
    }

    if (resolutionCache.TryGetValue(serviceType, out var service))
    {
      return service;
    }

    service = provider.GetService(serviceType);

    if (service != null)
    {
      resolutionCache[serviceType] = service;
    }

    return service;
  }

  public void Dispose()
  {
    provider?.Dispose();
  }

  private void CheckNotSealed()
  {
    if (isSealed)
    {
      throw new InvalidOperationException("The service registry is now sealed and new services can no longer be added");
    }
  }

  private static Microsoft.Extensions.DependencyInjection.ServiceLifetime ConvertLifetime(ServiceLifetime lifetime)
  {
    return lifetime switch
    {
      ServiceLifetime.Transient => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient,
      ServiceLifetime.Singleton => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton,

      _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null),
    };
  }
}
