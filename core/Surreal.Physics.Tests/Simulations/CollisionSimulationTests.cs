using Surreal.Timing;

namespace Surreal.Physics.Simulations;

public class CollisionSimulationTests
{
  [Test]
  public void it_should_simulate_objects_by_applying_gravity()
  {
    var simulation = new CollisionSimulation();

    simulation.Add(new Sphere());
    simulation.Add(new Plane());

    simulation.Update(16.Milliseconds());

    // TODO: assert objects have collided?
  }

  private sealed record Sphere : ICollisionObject
  {
    public Vector3 Position { get; set; }
    public float   Radius   { get; set; }

    public bool CollidesWith(ICollisionObject other, out CollisionDetails details)
    {
      switch (other)
      {
        case Sphere sphere:
        {
          throw new NotImplementedException();
        }
        case Plane plane:
        {
          throw new NotImplementedException();
        }
        default:
        {
          details = default;
          return false;
        }
      }
    }
  }

  private sealed record Plane : ICollisionObject
  {
    public Vector3 Position { get; set; }
    public Vector3 Normal   { get; set; }

    public bool CollidesWith(ICollisionObject other, out CollisionDetails details)
    {
      switch (other)
      {
        case Sphere sphere:
        {
          throw new NotImplementedException();
        }
        case Plane plane:
        {
          throw new NotImplementedException();
        }
        default:
        {
          details = default;
          return false;
        }
      }
    }
  }
}
