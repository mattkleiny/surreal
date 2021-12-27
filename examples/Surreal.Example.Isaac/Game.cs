using Surreal;

namespace Isaac;

public sealed class Game : PrototypeGame
{
	public static Task Main() => StartAsync<Game>(new()
	{
		Platform = new DesktopPlatform
		{
			Configuration =
			{
				Title = "The Binding of Isaac",
				IsVsyncEnabled = true,
				ShowFpsInTitle = true
			}
		}
	});
}
