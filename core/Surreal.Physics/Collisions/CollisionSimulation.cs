using Surreal.Timing;

namespace Surreal.Physics.Collisions;

/// <summary>A simple collision simulation against <see cref="ICollisionObject"/>s.</summary>
public class CollisionSimulation
{
  private readonly List<ICollisionObject> objects = new();

  public event Action<CollisionDetails>? CollisionDetected;

  public void Add(ICollisionObject value) => objects.Add(value);
  public void Remove(ICollisionObject value) => objects.Remove(value);

  public void Update(DeltaTime deltaTime)
  {
    // TODO: broadphase reduction

    for (var i = 0; i < objects.Count; i++)
    for (var j = 0; j < objects.Count; j++)
    {
      if (i == j) break; // don't compare same objects

      var object1 = objects[i];
      var object2 = objects[j];

      if (object1.CollidesWith(object2, out var details))
      {
        CollisionDetected?.Invoke(details);
      }
    }
  }
}
