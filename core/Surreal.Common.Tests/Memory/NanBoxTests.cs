using Surreal.Memory;

namespace Surreal;

public class NanBoxTests
{
  [Test]
  public void It_Should_Create_Bool()
  {
    Assert.IsTrue(NanBox.FromBool(true).IsTrue);
  }
}
