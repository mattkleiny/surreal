namespace Surreal.Memory;

public class SpanListTests
{
  [Test]
  public void it_should_read_and_write_to_list()
  {
    var list = new SpanList<ushort>(stackalloc ushort[256]);

    for (int i = 0; i < 256; i++)
    {
      list.Add(100);
    }
  }

  [Test]
  public void it_should_create_a_sub_list()
  {
    var list = new SpanList<ushort>(stackalloc ushort[8]);

    list.Add(1);
    list.Add(2);

    Assert.AreEqual(2, list[..].Count);
    Assert.AreEqual(1, list[1..].Count);
    Assert.AreEqual(0, list[2..].Count);
    Assert.AreEqual(0, list[3..].Count);
  }
}
