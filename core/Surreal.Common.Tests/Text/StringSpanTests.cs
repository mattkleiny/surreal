using NUnit.Framework;

namespace Surreal.Text;

public class StringSpanTests
{
  [Test]
  public void it_should_index_into_source_string()
  {
    var span = "Test".AsStringSpan();

    Assert.AreEqual('T', span[0]);
    Assert.AreEqual('t', span[^1]);
  }

  [Test]
  public void it_should_range_into_source_string()
  {
    var span = "Hello, World!".AsStringSpan();

    Assert.AreEqual("World!", span[7..].ToString());
  }
}
