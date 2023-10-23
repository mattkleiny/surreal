using Surreal.Memory;

namespace Surreal.Common.Tests.Memory;

public class SpanStackTests
{
  [Test]
  public void it_should_read_and_write_to_stack()
  {
    var stack = new SpanStack<ushort>(stackalloc ushort[256]);

    for (var i = 0; i < 256; i++)
    {
      stack.TryPush(100).Should().BeTrue();
    }

    stack.TryPush(200).Should().BeFalse();

    for (var i = 0; i < 256; i++)
    {
      stack.TryPop(out _).Should().BeTrue();
    }

    stack.TryPop(out _).Should().BeFalse();
  }

  [Test]
  public void it_should_create_a_sub_stack()
  {
    var stack = new SpanStack<ushort>(stackalloc ushort[8]);

    stack.Push(1);
    stack.Push(2);

    stack[..].Count.Should().Be(2);
    stack[1..].Count.Should().Be(1);
    stack[2..].Count.Should().Be(0);
    stack[3..].Count.Should().Be(0);
  }
}
