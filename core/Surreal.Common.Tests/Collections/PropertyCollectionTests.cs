namespace Surreal.Collections;

public class PropertyCollectionTests
{
  private static Property<string> Message { get; } = new(nameof(Message));
  private static Property<float>  Factor  { get; } = new(nameof(Factor), 1.14159f);

  [Test]
  public void it_should_read_and_write_keys()
  {
    var blackboard = new PropertyBag();

    blackboard.Set(Message, "Hello, World!");
    blackboard.Set(Factor, MathF.PI);

    blackboard.Get(Message).Should().Be("Hello, World!");
    blackboard.Get(Factor, 0.1f).Should().Be(MathF.PI);
  }

  [Test]
  public void it_should_favor_default_values_in_hierarchy()
  {
    var blackboard = new PropertyBag();

    blackboard.Get(Message, "Test").Should().Be("Test");
    blackboard.Get(Factor).Should().Be(1.14159f);
    blackboard.Get(Factor, 3.14159f).Should().Be(3.14159f);
  }
}
