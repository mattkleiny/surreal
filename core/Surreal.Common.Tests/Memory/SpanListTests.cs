namespace Surreal.Memory;

public class SpanListTests
{
  [Test]
  public void it_should_read_and_write_to_list()
  {
    var stack = new SpanList<ushort>(stackalloc ushort[256]);

    for (int i = 0; i < 256; i++)
    {
      stack.Add(100);
    }
  }
}
