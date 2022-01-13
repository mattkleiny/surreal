using Surreal.Timing;

namespace Surreal.Physics.Simulations;

/// <summary>A simple dynamics simulation against <see cref="IDynamicObject"/>s.</summary>
public class DynamicsSimulation
{
  private readonly List<IDynamicObject> objects = new();

  public Vector3 Gravity { get; set; } = new(0f, -9.8f, 0f);

  public void Add(IDynamicObject value)    => objects.Add(value);
  public void Remove(IDynamicObject value) => objects.Remove(value);

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
