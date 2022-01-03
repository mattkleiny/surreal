using NUnit.Framework;

namespace Surreal;

public class IdentifierTests
{
  [Test]
  public void it_should_allocate()
  {
    var id1 = Identifier.Allocate();
    var id2 = Identifier.Allocate();

    Assert.AreNotEqual(id1, id2);
  }
}
