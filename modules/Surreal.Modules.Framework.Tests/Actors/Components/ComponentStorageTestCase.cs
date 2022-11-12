using Surreal.Collections;

namespace Surreal.Components;

public record struct TestComponent(Vector2 Position = default, float Rotation = 0f)
{
  public Vector2 Position = Position;
  public float Rotation = Rotation;
}

public class ComponentStorageTestCase<TStorage>
  where TStorage : IComponentStorage<TestComponent>, new()
{
  [Test]
  public void it_should_add_components()
  {
    var arena = new GenerationalArena<Guid>();

    var entity1 = arena.Add(Guid.NewGuid());
    var entity2 = arena.Add(Guid.NewGuid());

    var positions = new DenseComponentStorage<TestComponent>();

    positions.AddComponent(entity1, new TestComponent(new Vector2(50, 20)));
    positions.AddComponent(entity2, new TestComponent(new Vector2(-50, -20)));
  }

  [Test]
  public void it_should_read_and_write_components()
  {
    var arena = new GenerationalArena<Guid>();

    var entity1 = arena.Add(Guid.NewGuid());
    var entity2 = arena.Add(Guid.NewGuid());

    var positions = new DenseComponentStorage<TestComponent>();

    positions.AddComponent(entity1, new TestComponent(new Vector2(50, 20)));
    positions.AddComponent(entity2, new TestComponent(new Vector2(-50, -20)));

    ref var component1 = ref positions.GetComponent(entity1);
    ref var component2 = ref positions.GetComponent(entity2);

    component1.Position.X += 10f;
    component2.Position.Y -= 10f;

    Assert.AreEqual(new Vector2(60, 20), component1.Position);
    Assert.AreEqual(new Vector2(-50, -30), component2.Position);
  }
}

[TestFixture]
public class DenseComponentStorageTests : ComponentStorageTestCase<DenseComponentStorage<TestComponent>>
{
}

[TestFixture]
public class SparseComponentStorageTests : ComponentStorageTestCase<SparseComponentStorage<TestComponent>>
{
}


