namespace Surreal.Graphics.Rendering;

public class RenderPassListTests
{
  [Test]
  public void it_should_insert_before_other_pass()
  {
    var list = new RenderPassList
    {
      new TestPass1(),
      new TestPass2()
    };

    list.InsertBefore<TestPass2>(new TestPass3());

    list[1].Should().BeOfType<TestPass3>();
  }

  [Test]
  public void it_should_insert_after_other_pass()
  {
    var list = new RenderPassList
    {
      new TestPass1(),
      new TestPass2()
    };

    list.InsertAfter<TestPass1>(new TestPass3());

    list[1].Should().BeOfType<TestPass3>();
  }

  private sealed class TestPass1 : RenderPass;
  private sealed class TestPass2 : RenderPass;
  private sealed class TestPass3 : RenderPass;
}
