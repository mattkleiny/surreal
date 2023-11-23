using Surreal.Services;

namespace Surreal.Physics;

/// <summary>
/// A <see cref="IServiceModule"/> for the physics system.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PhysicsModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService<IPhysicsBackend>(new PhysicsBackend());
  }
}
