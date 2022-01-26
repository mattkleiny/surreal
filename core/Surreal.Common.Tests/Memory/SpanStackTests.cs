namespace Surreal.Memory;

public class SpanStackTests
{
  [Test]
  public void it_should_read_and_write_to_stack()
  {
    var stack = new SpanStack<ushort>(stackalloc ushort[256]);

    for (int i = 0; i < 256; i++)
    {
      Assert.IsTrue(stack.TryPush(100));
    }

    Assert.IsFalse(stack.TryPush(200));

    for (int i = 0; i < 256; i++)
    {
      Assert.IsTrue(stack.TryPop(out _));
    }

    Assert.IsFalse(stack.TryPop(out _));
  }
}
