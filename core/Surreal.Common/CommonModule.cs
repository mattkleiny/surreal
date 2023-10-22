using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A module that registers all services in the common assembly.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class CommonModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddAssemblyServices(Assembly.GetExecutingAssembly());
  }
}
