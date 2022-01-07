﻿using NUnit.Framework.Internal;
using Prelude;

namespace Surreal;

public class PreludeGameTests : GameTestCase<PreludeGame>
{
  [Test]
  public async Task it_should_bootstrap_and_tick()
  {
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
          Title          = "Prelude Tests",
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
