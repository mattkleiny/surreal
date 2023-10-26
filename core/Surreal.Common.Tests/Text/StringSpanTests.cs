namespace Surreal.Text;

public class StringSpanTests
{
  [Test]
  public void it_should_index_into_source_string()
  {
    var span = "Test".AsStringSpan();

    span[0].Should().Be('T');
    span[^1].Should().Be('t');
  }

  [Test]
  public void it_should_range_into_source_string()
  {
    var span = "Hello, World!".AsStringSpan();

    span[7..].ToString().Should().Be("World!");
  }
}
