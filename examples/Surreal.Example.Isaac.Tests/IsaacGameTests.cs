using Isaac;
using NUnit.Framework;

namespace Surreal;

public class IsaacGameTests : GameTestCase<IsaacGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Tick(16.Milliseconds());
  }
}
