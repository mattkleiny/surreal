namespace Surreal.Collections.Immutable;

public class ImmutableWeightedListTests
{
  [Test]
  public void it_should_construct_from_flat_list()
  {
    var names = ImmutableWeightedList.Create(
      ("Bob", 0.1f),
      ("Alice", 0.2f),
      ("Charlie", 0.3f)
    );

    names.Count.Should().Be(3);
  }
}
