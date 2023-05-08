namespace Surreal.Services;

/// <summary>
/// A registry for services.
/// </summary>
public interface IServiceRegistry : IServiceProvider, IDisposable
{
  void RegisterService(Type serviceType, object instance);

  /// <summary>
  /// Adds the given <see cref="TService"/> to the registry.
  /// </summary>
  void AddService<TService>(TService implementation)
    where TService : class
  {
    RegisterService(typeof(TService), implementation);
  }

  /// <summary>
  /// Adds the given <see cref="IServiceModule" /> to the registry.
  /// </summary>
  void AddModule(IServiceModule module)
  {
    module.RegisterServices(this);
  }

  /// <summary>
  /// Registers all of the <see cref="RegisterServiceAttribute" />-annotated types in the given assembly.
  /// </summary>
  [RequiresUnreferencedCode("Discovers services via reflection")]
  void AddAssemblyServices(Assembly assembly)
  {
    var candidates =
      from type in assembly.GetTypes()
      from attribute in type.GetCustomAttributes<RegisterServiceAttribute>(true)
      select new { Attribute = attribute, Type = type };

    foreach (var candidate in candidates)
    {
      candidate.Attribute.RegisterService(candidate.Type, this);
    }
  }
}
