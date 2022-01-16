namespace Surreal.Utilities;

public class ValueComparisonTests
{
  [Test]
  public void it_should_compare_less_than()
  {
    var comparison = new ValueComparison<int>(ComparisonType.LessThan, 5);

    Assert.IsTrue(comparison.Compare(3));
    Assert.IsFalse(comparison.Compare(5));
    Assert.IsFalse(comparison.Compare(7));
  }

  [Test]
  public void it_should_compare_less_than_or_equal()
  {
    var comparison = new ValueComparison<int>(ComparisonType.LessThanOrEqual, 5);

    Assert.IsTrue(comparison.Compare(3));
    Assert.IsTrue(comparison.Compare(5));
    Assert.IsFalse(comparison.Compare(7));
  }

  [Test]
  public void it_should_compare_equal()
  {
    var comparison = new ValueComparison<int>(ComparisonType.EqualTo, 5);

    Assert.IsFalse(comparison.Compare(3));
    Assert.IsTrue(comparison.Compare(5));
    Assert.IsFalse(comparison.Compare(7));
  }

  [Test]
  public void it_should_compare_greater_than()
  {
    var comparison = new ValueComparison<int>(ComparisonType.GreaterThan, 5);

    Assert.IsFalse(comparison.Compare(3));
    Assert.IsFalse(comparison.Compare(5));
    Assert.IsTrue(comparison.Compare(7));
  }

  [Test]
  public void it_should_compare_greater_than_or_equal()
  {
    var comparison = new ValueComparison<int>(ComparisonType.GreaterThanOrEqual, 5);

    Assert.IsFalse(comparison.Compare(3));
    Assert.IsTrue(comparison.Compare(5));
    Assert.IsTrue(comparison.Compare(7));
  }
}
