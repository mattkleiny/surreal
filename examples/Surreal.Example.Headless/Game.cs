using Headless.Screens;
using Surreal;
using Surreal.Diagnostics;
using Surreal.Diagnostics.Logging;
using Surreal.Framework;
using Surreal.Mathematics.Timing;
using Surreal.Platform;

namespace Headless {
  public sealed class Game : GameJam<Game> {
    private static readonly ILog Log = LogFactory.GetLog<Game>();

    private readonly FrameReporter frameReporter = new FrameReporter(Log, interval: 1.Seconds());

    public static void Main() => Start<Game>(new Configuration {
        Platform = new HeadlessPlatform(),
    });

    public new IHeadlessPlatformHost Host => (IHeadlessPlatformHost) base.Host;

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));
    }

    protected override void Draw(GameTime time) {
      base.Draw(time);

      frameReporter.Tick(time.DeltaTime);
    }
  }
}