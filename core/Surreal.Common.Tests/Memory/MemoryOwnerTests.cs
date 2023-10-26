namespace Surreal.Memory;

public class MemoryOwnerTests
{
  [Test]
  public void it_should_allocate_and_fill()
  {
    using var owner = MemoryOwner<int>.Allocate(32);

    owner.Span.Fill(42);

    Assert.AreEqual(32, owner.Length);
  }
}
