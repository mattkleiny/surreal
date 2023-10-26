namespace Surreal.Memory;

public class SpanOwnerTests
{
  [Test]
  public void it_should_allocate_and_fill()
  {
    using var owner = SpanOwner<int>.Allocate(32);

    owner.Span.Fill(42);

    Assert.AreEqual(32, owner.Length);
  }
}
