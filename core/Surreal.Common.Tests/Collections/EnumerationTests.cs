using System.Diagnostics.CodeAnalysis;

namespace Surreal.Collections;

public class EnumerationTests
{
  [Test]
  public void it_should_build_a_valid_set_of_all_members()
  {
    TestValue.All.Count.Should().Be(3);
    TestValue.All.Should().BeEquivalentTo(new[]
    {
      TestValue.One,
      TestValue.Two,
      TestValue.Three,
    });
  }

  [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local")]
  private sealed record TestValue(int Id) : Enumeration<TestValue>
  {
    public static TestValue One   { get; } = new(1);
    public static TestValue Two   { get; } = new(2);
    public static TestValue Three { get; } = new(3);
  }
}
