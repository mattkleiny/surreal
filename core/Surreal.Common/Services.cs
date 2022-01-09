using System.Reflection;
using JetBrains.Annotations;

namespace Surreal;

/// <summary>Abstractions over service lifetime scopes.</summary>
public enum ServiceLifetime
{
  Transient,
  Singleton,
}

/// <summary>A module for service registrations.</summary>
public interface IServiceModule
{
  void RegisterServices(IServiceRegistry services);
}

/// <summary>A registry for services.</summary>
public interface IServiceRegistry : IServiceProvider, IDisposable
{
  void RegisterService(ServiceLifetime lifetime, Type serviceType, Type implementationType);
  void RegisterService(Type serviceType, object instance);
  void ReplaceService(ServiceLifetime lifetime, Type serviceType, Type implementationType);
  void ReplaceService(Type serviceType, object instance);

  /// <summary>Prevents new services from being installed beyond this point.</summary>
  void SealRegistry();

  void AddTransient<TService, TImplementation>()
    where TImplementation : TService
  {
    RegisterService(ServiceLifetime.Transient, typeof(TService), typeof(TImplementation));
  }

  void AddSingleton<TService, TImplementation>()
    where TImplementation : TService
  {
    RegisterService(ServiceLifetime.Singleton, typeof(TService), typeof(TImplementation));
  }

  void AddSingleton<TService>(TService implementation)
    where TService : class
  {
    RegisterService(typeof(TService), implementation);
  }

  void ReplaceTransient<TService, TImplementation>()
    where TService : class
  {
    ReplaceService(ServiceLifetime.Transient, typeof(TService), typeof(TImplementation));
  }

  void ReplaceSingleton<TService, TImplementation>()
    where TService : class
  {
    ReplaceService(ServiceLifetime.Singleton, typeof(TService), typeof(TImplementation));
  }

  void ReplaceSingleton<TService>(TService implementation)
    where TService : class
  {
    ReplaceService(typeof(TService), implementation);
  }

  /// <summary>Adds the given <see cref="IServiceModule"/> to the registry.</summary>
  void AddModule(IServiceModule module)
  {
    module.RegisterServices(this);
  }

  /// <summary>Registers all of the <see cref="RegisterServiceAttribute"/>-annotated types in the given assembly.</summary>
  void AddAssemblyServices(Assembly assembly)
  {
    var candidates =
      from type in assembly.GetTypes()
      from attribute in type.GetCustomAttributes<RegisterServiceAttribute>(inherit: true)
      select new { Attribute = attribute, Type = type };

    foreach (var candidate in candidates)
    {
      candidate.Attribute.RegisterService(candidate.Type, this);
    }
  }
}

/// <summary>Static extension methods for <see cref="IServiceProvider"/> and related.</summary>
public static class ServicesExtensions
{
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
      result = (T) service;
      return true;
    }

    result = default!;
    return false;
  }
}

/// <summary>Exports a type as the implementation of some service, for use in auto-discovery.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RegisterServiceAttribute : Attribute
{
  public Type?           ServiceType { get; }
  public ServiceLifetime Lifetime    { get; set; } = ServiceLifetime.Singleton;

  public RegisterServiceAttribute(Type? serviceType = null)
  {
    ServiceType = serviceType;
  }

  public void RegisterService(Type type, IServiceRegistry registry)
  {
    registry.RegisterService(Lifetime, ServiceType ?? type, type);
  }
}

/// <summary>Indicates a service is not available.</summary>
public class ServiceNotFoundException : Exception
{
  public ServiceNotFoundException(string message)
    : base(message)
  {
  }
}
