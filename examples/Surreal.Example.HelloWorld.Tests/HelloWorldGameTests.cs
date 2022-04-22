namespace HelloWorld;

public class HelloWorldGameTests : GameTestCase<HelloWorldGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Run(1.Seconds());
  }
}
