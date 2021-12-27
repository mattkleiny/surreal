using Surreal;
using Surreal.Diagnostics;
using Surreal.Diagnostics.Logging;
using Surreal.Timing;
using Timer = Surreal.Timing.Timer;

namespace Headless;

public sealed class Game : PrototypeGame
{
	private static readonly ILog Log = LogFactory.GetLog<Game>();

	private readonly FpsCounter fpsCounter = new();
	private Timer fpsTimer = new(1.Seconds());

	public static Task Main() => StartAsync<Game>(new Configuration
	{
		Platform = new HeadlessPlatform()
	});

	protected override void Draw(GameTime time)
	{
		base.Draw(time);

		if (fpsTimer.Tick(time.DeltaTime))
		{
			Log.Trace($"Frames per second: {fpsCounter.FramesPerSecond:F}");
		}

		fpsCounter.Tick(time.DeltaTime);
	}
}
