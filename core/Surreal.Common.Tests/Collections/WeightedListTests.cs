namespace Surreal.Collections;

public class WeightedListTests
{
  [Test]
  public void it_should_produce_a_list_of_weighted_elements()
  {
    var list = new WeightedList<string>();

    list.Add("A", 1);
    list.Add("B", 2);
    list.Add("C", 3);

    list.TrySelect(out var result).Should().BeTrue();

    result.Should().BeOneOf("A", "B", "C");
  }
}
