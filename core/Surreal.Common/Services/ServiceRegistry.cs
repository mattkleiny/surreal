using Surreal.Collections;
using Surreal.Collections.Slices;
using Surreal.Diagnostics.Logging;

namespace Surreal.Services;

/// <summary>
/// Abstracts over a service registry, allowing different IoC container depending on game host.
/// </summary>
public interface IServiceRegistry : IServiceProvider
{
  /// <summary>
  /// Attempts to get a service of the given type. If the service is not found, returns <c>false</c>.
  /// </summary>
  bool TryGetService(Type serviceType, [MaybeNullWhen(false)] out object instance);

  /// <summary>
  /// Attempts to get all services of the given type. If the service is not found, returns an empty enumerable.
  /// </summary>
  IEnumerable<object> GetServices(Type serviceType);

  /// <summary>
  /// Registers a service.
  /// </summary>
  void AddService(Type serviceType, object instance);

  /// <summary>
  /// Registers a service.
  /// </summary>
  void AddService(Type serviceType, Type implType, ServiceLifetime lifetime);

  /// <summary>
  /// Instantiates a type and populates its service without adding it to the container.
  /// </summary>
  object Instantiate(Type serviceType);

  /// <summary>
  /// Invokes the given <see cref="Delegate"/> with services available in the container, as
  /// well as the auxiliary services provided in <see cref="extraServices"/>
  /// </summary>
  object? ExecuteDelegate(Delegate @delegate, params object[] extraServices);
}

/// <summary>
/// A simple default <see cref="IServiceRegistry" /> implementation.
/// </summary>
public sealed class ServiceRegistry : IServiceRegistry, IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<ServiceRegistry>();

  private readonly List<IDisposable> _orderedDisposables = [];
  private readonly List<ServiceRegistration> _registrations = [];
  private readonly MultiDictionary<Type, object> _container = new();

  /// <summary>
  /// Resolves the service of the given type.
  /// </summary>
  private bool TryResolveService(Type serviceType, out object? result)
  {
    if (_container.TryGetValues(serviceType, out var services))
    {
      result = services[0];
      return true;
    }

    for (var i = 0; i < _registrations.Count; i++)
    {
      var registration = _registrations[i];
      if (registration.ServiceType == serviceType)
      {
        var instance = Instantiate(registration.ImplementationType);
        if (registration.Lifetime == ServiceLifetime.Singleton)
        {
          _container.Add(serviceType, instance);
          _registrations.RemoveAt(i);

          // remember the order in which services were added, so we can dispose them in reverse order
          if (instance is IDisposable disposable)
          {
            _orderedDisposables.Add(disposable);
          }
        }

        result = instance;
        return true;
      }
    }

    result = default;
    return false;
  }

  /// <summary>
  /// Resolves the services of the given type.
  /// </summary>
  private IEnumerable<object> ResolveServices(Type serviceType)
  {
    if (_container.TryGetValues(serviceType, out var services))
    {
      foreach (var service in services)
      {
        yield return service;
      }
    }

    for (var i = _registrations.Count - 1; i >= 0; i--)
    {
      var registration = _registrations[i];
      if (registration.ServiceType == serviceType)
      {
        var instance = Instantiate(registration.ImplementationType);
        if (registration.Lifetime == ServiceLifetime.Singleton)
        {
          _container.Add(serviceType, instance);
          _registrations.RemoveAt(i);

          // remember the order in which services were added, so we can dispose them in reverse order
          if (instance is IDisposable disposable)
          {
            _orderedDisposables.Add(disposable);
          }
        }

        yield return instance;
      }
    }
  }

  public object? GetService(Type serviceType)
  {
    if (!TryResolveService(serviceType, out var service))
    {
      return default;
    }

    return service;
  }

  public IEnumerable<object> GetServices(Type serviceType)
  {
    return ResolveServices(serviceType);
  }

  public bool TryGetService(Type serviceType, [MaybeNullWhen(false)] out object instance)
  {
    return TryResolveService(serviceType, out instance);
  }

  public void AddService(Type serviceType, object instance)
  {
    Log.Trace($"Registering service {serviceType} with instance {instance}");

    _container.Add(serviceType, instance);

    // remember the order in which services were added, so we can dispose them in reverse order
    if (instance is IDisposable disposable)
    {
      _orderedDisposables.Add(disposable);
    }
  }

  public void AddService(Type serviceType, Type implType, ServiceLifetime lifetime)
  {
    Log.Trace($"Registering service {serviceType} with implementation {implType} and lifetime {lifetime}");

    _registrations.Add(new ServiceRegistration
    {
      ServiceType = serviceType,
      ImplementationType = implType,
      Lifetime = lifetime
    });
  }

  /// <summary>
  /// Invokes the given <see cref="Delegate"/> with services available in the container, as
  /// well as the auxiliary services provided in <see cref="extraServices"/>
  /// </summary>
  public object? ExecuteDelegate(Delegate @delegate, params object[] extraServices)
  {
    var methodInfo = @delegate.GetMethodInfo();
    var parameterInfos = methodInfo.GetParameters();
    var parameters = new object[parameterInfos.Length];
    var servicesByType = extraServices.ToDictionary(s => s.GetType());

    for (var i = 0; i < parameterInfos.Length; i++)
    {
      var parameterInfo = parameterInfos[i];
      var parameterType = parameterInfo.ParameterType;

      if (TryGetService(parameterType, out var instance))
      {
        parameters[i] = instance;
      }
      else if (servicesByType.TryGetValue(parameterType, out instance))
      {
        parameters[i] = instance;
      }
      else
      {
        throw new InvalidOperationException($"Unable to resolve parameter {parameterInfo}");
      }
    }

    return methodInfo.Invoke(@delegate.Target, parameters);
  }

  public object Instantiate(Type serviceType)
  {
    var constructors = serviceType
      .GetConstructors()
      .Where(c => c.IsPublic)
      .OrderByDescending(c => c.GetParameters().Length);

    var candidates = new List<ConstructorInfo>(constructors);

    for (var i = candidates.Count - 1; i >= 0; i--)
    {
      foreach (var parameter in candidates[i].GetParameters())
      {
        if (!_container.ContainsKey(parameter.ParameterType))
        {
          candidates.RemoveAt(i);
          break;
        }
      }
    }

    if (candidates.Count == 0)
    {
      return Activator.CreateInstance(serviceType)!;
    }

    var constructorInfo = candidates[0];
    var parameterInfos = constructorInfo.GetParameters();
    var parameters = new object[parameterInfos.Length];

    for (var i = 0; i < parameterInfos.Length; i++)
    {
      parameters[i] = _container[parameterInfos[i].ParameterType].FirstOrDefault()!;
    }

    return constructorInfo.Invoke(parameters);
  }

  public void Dispose()
  {
    for (var i = _orderedDisposables.Count - 1; i >= 0; i--)
    {
      _orderedDisposables[i].Dispose();
    }

    _orderedDisposables.Clear();
    _container.Clear();
  }

  /// <summary>
  /// A single registration for a type in the registry.
  /// </summary>
  private sealed class ServiceRegistration
  {
    public required Type ServiceType { get; init; }
    public required Type ImplementationType { get; init; }

    /// <summary>
    /// The lifetime to assign to the created service.
    /// </summary>
    public required ServiceLifetime Lifetime { get; init; }
  }
}
