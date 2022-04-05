namespace Surreal;

public class IdentifierTests
{
  [Test]
  public void it_should_allocate()
  {
    var id1 = Identifier.Allocate<object>();
    var id2 = Identifier.Allocate<object>();

    id1.Should().NotBe(id2);
  }
}
