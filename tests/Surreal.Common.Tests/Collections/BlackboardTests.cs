using NUnit.Framework;

namespace Surreal.Collections;

public class BlackboardTests
{
  private static BlackboardProperty<string> Message { get; } = new(nameof(Message));
  private static BlackboardProperty<float>  Factor  { get; } = new(nameof(Factor), 1.14159f);

  [Test]
  public void it_should_read_and_write_keys()
  {
    var blackboard = new Blackboard();

    blackboard.Set(Message, "Hello, World!");
    blackboard.Set(Factor, MathF.PI);

    Assert.AreEqual("Hello, World!", blackboard.Get(Message));
    Assert.AreEqual(MathF.PI, blackboard.Get(Factor), 0.1f);
  }

  [Test]
  public void it_should_favor_default_values_in_hierarchy()
  {
    var blackboard = new Blackboard();

    Assert.AreEqual("Test", blackboard.Get(Message, "Test"));
    Assert.AreEqual(1.14159f, blackboard.Get(Factor));
    Assert.AreEqual(3.14159f, blackboard.Get(Factor, 3.14159f));
  }
}