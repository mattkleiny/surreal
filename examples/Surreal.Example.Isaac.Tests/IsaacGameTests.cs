﻿using Isaac;
using NUnit.Framework.Internal;

namespace Surreal;

public class IsaacGameTests : GameTestCase<IsaacGame>
{
  [Test]
  public async Task it_should_bootstrap_and_tick()
  {
    await GameUnderTest.InitializeAsync();
    await GameUnderTest.RunAsync(1.Seconds());
  }

  protected override IPlatform CreatePlatform()
  {
    if (OSPlatform.CurrentPlatform.IsWindows)
    {
      return new DesktopPlatform
      {
        Configuration =
        {
          Title          = "Isaac Tests",
          Width          = 256,
          Height         = 144,
          IsResizable    = false,
          ShowFpsInTitle = false,
        }
      };
    }

    return base.CreatePlatform();
  }
}
