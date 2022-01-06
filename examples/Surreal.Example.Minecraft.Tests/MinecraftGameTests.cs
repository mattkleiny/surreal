using Minecraft;
using NUnit.Framework;

namespace Surreal;

public class MinecraftGameTests : GameTestCase<MinecraftGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Tick(16.Milliseconds());
  }
}
