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

  [Test]
  public void it_should_create_a_sub_stack()
  {
    var stack = new SpanStack<ushort>(stackalloc ushort[8]);

    stack.Push(1);
    stack.Push(2);

    Assert.AreEqual(2, stack[..].Count);
    Assert.AreEqual(1, stack[1..].Count);
    Assert.AreEqual(0, stack[2..].Count);
    Assert.AreEqual(0, stack[3..].Count);
  }
}
