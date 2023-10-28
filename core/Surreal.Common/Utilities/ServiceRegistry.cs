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

  /// <summary>
  /// Instantiates a type and populates it's service without adding it to the container.
  /// </summary>
  T Instantiate<T>() where T : class;

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

  private readonly ServiceContainer _container = new();

  public object? GetService(Type serviceType)
  {
    return _container.TryGetInstance(serviceType);
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

      if (_container.CanGetInstance(parameterType, null))
      {
        parameters[i] = _container.GetInstance(parameterType);
      }
      else if (servicesByType.TryGetValue(parameterType, out var instance))
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

  public T Instantiate<T>()
    where T : class
  {
    var constructors = typeof(T).GetConstructors()
      .Where(c => c.IsPublic)
      .OrderByDescending(c => c.GetParameters().Length);

    var candidates = new List<ConstructorInfo>(constructors);

    for (var i = candidates.Count - 1; i >= 0; i--)
    {
      foreach (var parameter in candidates[i].GetParameters())
      {
        if (!_container.CanGetInstance(parameter.ParameterType, null))
        {
          candidates.RemoveAt(i);
          break;
        }
      }
    }

    if (candidates.Count == 0)
    {
      return Activator.CreateInstance<T>();
    }

    var constructorInfo = candidates[0];
    var parameterInfos = constructorInfo.GetParameters();
    var parameters = new object[parameterInfos.Length];

    for (var i = 0; i < parameterInfos.Length; i++)
    {
      parameters[i] = _container.GetInstance(parameterInfos[i].ParameterType);
    }

    return (T)constructorInfo.Invoke(parameters);
  }

  public void Dispose()
  {
    _container.Dispose();
  }

  private static string GenerateName() => Guid.NewGuid().ToString();
}
