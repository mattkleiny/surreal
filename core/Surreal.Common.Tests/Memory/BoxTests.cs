namespace Surreal.Memory;

public class BoxTests
{
  [Test]
  public void it_should_behave_like_a_boxed_value_type()
  {
    Box<int> value = 42;

    Assert.AreEqual(42, value.GetReference());
  }
}
