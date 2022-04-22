namespace Minecraft;

public class MinecraftGameTests : GameTestCase<MinecraftGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Run(1.Seconds());
  }
}
