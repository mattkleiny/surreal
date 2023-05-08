namespace Surreal.Memory;

public class SpanListTests
{
  [Test]
  public void it_should_read_and_write_to_list()
  {
    var list = new SpanList<ushort>(stackalloc ushort[256]);

    for (var i = 0; i < 256; i++)
    {
      list.Add(100);
    }

    list.Count.Should().Be(256);
  }

  [Test]
  public void it_should_create_a_sub_list()
  {
    var list = new SpanList<ushort>(stackalloc ushort[8]);

    list.Add(1);
    list.Add(2);

    list[..].Count.Should().Be(2);
    list[1..].Count.Should().Be(1);
    list[2..].Count.Should().Be(0);
    list[3..].Count.Should().Be(0);
  }
}
