using HelloWorld;

namespace Surreal;

public class HelloWorldGameTests : GameTestCase<HelloWorldGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Tick(16.Milliseconds());
  }
}
