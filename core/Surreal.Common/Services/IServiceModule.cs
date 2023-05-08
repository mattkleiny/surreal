namespace Surreal.Services;

/// <summary>
/// A module for service registrations.
/// </summary>
public interface IServiceModule
{
  void RegisterServices(IServiceRegistry services);
}