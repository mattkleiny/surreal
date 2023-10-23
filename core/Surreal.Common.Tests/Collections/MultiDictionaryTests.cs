using Surreal.Collections;

namespace Surreal.Common.Tests.Collections;

public class MultiDictionaryTests
{
  [Test]
  public void it_should_permit_multiple_values_per_key()
  {
    var dictionary = new MultiDictionary<int, string>();

    dictionary.Add(1, "Test 1");
    dictionary.Add(1, "Test 2");
    dictionary.Add(2, "Test 3");
    dictionary.Add(2, "Test 4");

    dictionary.KeyCount.Should().Be(2);
  }

  [Test]
  public void it_removing_an_item_should_remove_the_value()
  {
    var dictionary = new MultiDictionary<int, string>();

    dictionary.Add(1, "Test 1");
    dictionary.Add(1, "Test 2");
    dictionary.Add(2, "Test 3");
    dictionary.Add(2, "Test 4");

    dictionary.KeyCount.Should().Be(2);

    dictionary.Remove(1, "Test 2");

    dictionary.KeyCount.Should().Be(2);
  }
}
