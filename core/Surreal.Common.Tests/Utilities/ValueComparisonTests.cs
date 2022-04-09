namespace Surreal.Utilities;

public class ValueComparisonTests
{
  [Test]
  public void it_should_compare_less_than()
  {
    var comparison = new ValueComparison<int>(ComparisonType.LessThan, 5);

    comparison.Compare(3).Should().BeTrue();
    comparison.Compare(5).Should().BeFalse();
    comparison.Compare(7).Should().BeFalse();
  }

  [Test]
  public void it_should_compare_less_than_or_equal()
  {
    var comparison = new ValueComparison<int>(ComparisonType.LessThanOrEqual, 5);

    comparison.Compare(3).Should().BeTrue();
    comparison.Compare(5).Should().BeTrue();
    comparison.Compare(7).Should().BeFalse();
  }

  [Test]
  public void it_should_compare_equal()
  {
    var comparison = new ValueComparison<int>(ComparisonType.EqualTo, 5);

    comparison.Compare(3).Should().BeFalse();
    comparison.Compare(5).Should().BeTrue();
    comparison.Compare(7).Should().BeFalse();
  }

  [Test]
  public void it_should_compare_greater_than()
  {
    var comparison = new ValueComparison<int>(ComparisonType.GreaterThan, 5);

    comparison.Compare(3).Should().BeFalse();
    comparison.Compare(5).Should().BeFalse();
    comparison.Compare(7).Should().BeTrue();
  }

  [Test]
  public void it_should_compare_greater_than_or_equal()
  {
    var comparison = new ValueComparison<int>(ComparisonType.GreaterThanOrEqual, 5);

    comparison.Compare(3).Should().BeFalse();
    comparison.Compare(5).Should().BeTrue();
    comparison.Compare(7).Should().BeTrue();
  }
}
