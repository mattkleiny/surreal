using Surreal;

namespace Asteroids;

public sealed class Game : PrototypeGame
{
	public static Task Main() => StartAsync<Game>(new()
	{
		Platform = new DesktopPlatform
		{
			Configuration =
			{
				Title = "Asteroids",
				IsVsyncEnabled = true,
				ShowFpsInTitle = true
			}
		}
	});
}
