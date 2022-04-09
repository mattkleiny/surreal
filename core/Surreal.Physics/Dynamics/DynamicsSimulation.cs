using Surreal.Timing;

namespace Surreal.Physics.Dynamics;

/// <summary>A simple dynamics simulation against <see cref="IDynamicObject"/>s.</summary>
public class DynamicsSimulation
{
  private readonly List<IDynamicObject> objects = new();

  /// <summary>The default gravity to apply to all <see cref="IDynamicObject"/>s.</summary>
  public Vector3 Gravity { get; set; } = new(0f, -9.8f, 0f);

  public void Add(IDynamicObject value) => objects.Add(value);
  public void Remove(IDynamicObject value) => objects.Remove(value);

  /// <summary>Integrates all forces across our <see cref="IDynamicObject"/>s.</summary>
  public void Update(DeltaTime deltaTime)
  {
    foreach (var o in objects)
    {
      o.Force += o.Mass * Gravity;

      o.Velocity += o.Force / o.Mass * deltaTime;
      o.Position += o.Velocity * deltaTime;

      o.Force = Vector3.Zero;
    }
  }
}
