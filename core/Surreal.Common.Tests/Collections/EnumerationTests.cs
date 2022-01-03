using NUnit.Framework;

namespace Surreal.Collections;

public class EnumerationTests
{
  [Test]
  public void it_should_build_a_valid_set_of_all_members()
  {
    Assert.AreEqual(3, TestValue.All.Count);
    Assert.That(TestValue.All, Is.EquivalentTo(new[] { TestValue.One, TestValue.Two, TestValue.Three }));
  }

  private record TestValue(int Id) : Enumeration<TestValue>
  {
    public static TestValue One   { get; } = new(1);
    public static TestValue Two   { get; } = new(2);
    public static TestValue Three { get; } = new(3);
  }
}
