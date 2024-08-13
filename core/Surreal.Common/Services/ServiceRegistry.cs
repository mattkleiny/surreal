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
  private readonly MultiDictionary<Type, object> _container = new();

  public object? GetService(Type serviceType)
  {
    if (!_container.TryGetValues(serviceType, out var services))
    {
      return null;
    }

    return services[0];
  }

  public IEnumerable<object> GetServices(Type serviceType)
  {
    if (!_container.TryGetValues(serviceType, out var services))
    {
      return ReadOnlySlice<object>.Empty;
    }

    return services;
  }

  public bool TryGetService(Type serviceType, [MaybeNullWhen(false)] out object instance)
  {
    if (!_container.TryGetValues(serviceType, out var services))
    {
      instance = default;
      return false;
    }

    instance = services[0];
    return true;
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
}
