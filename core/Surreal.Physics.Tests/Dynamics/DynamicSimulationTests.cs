using Surreal.Timing;

namespace Surreal.Physics.Dynamics;

public class DynamicSimulationTests
{
  [Test]
  public void it_should_simulate_objects_by_applying_gravity()
  {
    var simulation = new DynamicsSimulation
    {
      Gravity = new Vector3(0f, -9.8f, 0f),
    };

    var sphere = new Sphere();

    simulation.Add(sphere);

    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());

    sphere.Position.Should().NotBe(Vector3.Zero);
  }

  [Test]
  public void it_should_simulate_objects_by_applying_velocity()
  {
    var simulation = new DynamicsSimulation
    {
      Gravity = Vector3.Zero
    };

    var sphere = new Sphere
    {
      Velocity = Vector3.UnitX * 2f
    };

    simulation.Add(sphere);

    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());
    simulation.Update(16.Milliseconds());

    sphere.Position.Should().NotBe(Vector3.Zero);
  }
}
