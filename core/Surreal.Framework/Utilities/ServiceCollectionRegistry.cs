using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Surreal.Utilities;

/// <summary>A <see cref="IServiceRegistry"/> that wraps a <see cref="IServiceCollection"/>.</summary>
internal sealed class ServiceCollectionRegistry : IServiceRegistry
{
  private readonly ServiceCollection services = new();

  public void RegisterService(ServiceLifetime lifetime, Type serviceType, Type implementationType)
  {
    services.Add(new ServiceDescriptor(serviceType, implementationType, ConvertLifetime(lifetime)));
  }

  public void RegisterService(Type serviceType, object instance)
  {
    services.Add(new ServiceDescriptor(serviceType, instance));
  }

  public IServiceProvider BuildServiceProvider()
  {
    return services.BuildServiceProvider();
  }

  private static Microsoft.Extensions.DependencyInjection.ServiceLifetime ConvertLifetime(ServiceLifetime lifetime)
  {
    return lifetime switch
    {
      ServiceLifetime.Transient => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient,
      ServiceLifetime.Singleton => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton,

      _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
    };
  }
}