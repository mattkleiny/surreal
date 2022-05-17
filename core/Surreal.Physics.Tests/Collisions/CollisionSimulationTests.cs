using Surreal.Timing;

namespace Surreal.Physics.Collisions;

public class CollisionSimulationTests
{
  [Test, Ignore("Not yet implemented")]
  public void it_should_simulate_objects_by_applying_gravity()
  {
    var simulation = new CollisionSimulation();

    simulation.Add(new Sphere());
    simulation.Add(new Plane());

    simulation.Update(16.Milliseconds());

    // TODO: assert objects have collided?
  }
}
