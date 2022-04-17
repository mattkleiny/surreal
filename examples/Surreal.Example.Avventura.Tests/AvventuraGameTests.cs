﻿namespace Avventura;

public class AvventuraGameTests : GameTestCase<AvventuraGame>
{
  [Test]
  public async Task it_should_bootstrap_and_tick()
  {
    await GameUnderTest.RunAsync(1.Seconds());
  }
}