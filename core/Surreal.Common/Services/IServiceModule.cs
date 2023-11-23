namespace Surreal.Services;

/// <summary>
/// Indicates a module that can be registered in a <see cref="IServiceRegistry"/>.
/// </summary>
public interface IServiceModule
{
  /// <summary>
  /// Registers all services in the module.
  /// </summary>
  void RegisterServices(IServiceRegistry registry);
}
