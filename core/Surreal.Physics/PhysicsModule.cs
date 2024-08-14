using Surreal.Services;

namespace Surreal.Physics;

/// <summary>
/// A <see cref="IServiceModule"/> for the physics system.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PhysicsModule : IServiceModule
{
  /// <summary>
  /// The <see cref="IPhysicsBackend"/> to be used.
  /// </summary>
  public IPhysicsBackend Backend { get; } = new PhysicsBackend();

  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService(Backend);
  }
}
