namespace Surreal.Collections;

public class PropertyCollectionTests
{
  private static Property<string> Message { get; } = new(nameof(Message));
  private static Property<float>  Factor  { get; } = new(nameof(Factor), 1.14159f);

  [Test]
  public void it_should_read_and_write_keys()
  {
    var blackboard = new PropertyCollection();

    blackboard.Set(Message, "Hello, World!");
    blackboard.Set(Factor, MathF.PI);

    Assert.AreEqual("Hello, World!", blackboard.Get(Message));
    Assert.AreEqual(MathF.PI, blackboard.Get(Factor), 0.1f);
  }

  [Test]
  public void it_should_favor_default_values_in_hierarchy()
  {
    var blackboard = new PropertyCollection();

    Assert.AreEqual("Test", blackboard.Get(Message, "Test"));
    Assert.AreEqual(1.14159f, blackboard.Get(Factor));
    Assert.AreEqual(3.14159f, blackboard.Get(Factor, 3.14159f));
  }
}
