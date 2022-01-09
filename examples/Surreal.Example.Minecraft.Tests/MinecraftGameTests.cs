using Minecraft;

namespace Surreal;

public class MinecraftGameTests : GameTestCase<MinecraftGame>
{
  [Test]
  public async Task it_should_bootstrap_and_tick()
  {
    await GameUnderTest.RunAsync(1.Seconds());
  }
}
