namespace Surreal.Collections.Immutable;

public class ImmutableMultiDictionaryTests
{
  [Test]
  public void it_should_construct_from_selectors()
  {
    var source = Enumerable.Range(0, 1000);
    var buckets = source.ToImmutableMultiDictionary(it => it % 2 == 0);

    buckets.KeyCount.Should().Be(2);
    buckets.ValueCount.Should().Be(1000);
  }
}
