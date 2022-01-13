using Surreal.Timing;

namespace Surreal.Physics.Simulations;

public class DynamicSimulationTests
{
  [Test]
  public void it_should_simulate_objects_by_applying_gravity()
  {
    var simulation = new DynamicsSimulation
    {
      Gravity = new Vector3(0f, -9.8f, 0f),
    };

    var value = new DynamicObject();

    simulation.Add(value);

    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());

    Assert.AreNotEqual(Vector3.Zero, value.Position);
  }

  [Test]
  public void it_should_simulate_objects_by_applying_velocity()
  {
    var simulation = new DynamicsSimulation
    {
      Gravity = Vector3.Zero
    };

    var value = new DynamicObject
    {
      Velocity = Vector3.UnitX * 2f
    };

    simulation.Add(value);

    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());

    Assert.AreNotEqual(Vector3.Zero, value.Position);
  }

  private sealed record DynamicObject : IDynamicObject
  {
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 force;

    public float Mass { get; set; } = 1f;

    public ref Vector3 Position => ref position;
    public ref Vector3 Velocity => ref velocity;
    public ref Vector3 Force    => ref force;
  }
}
