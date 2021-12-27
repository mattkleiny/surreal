using Surreal;

namespace HelloWorld;

public sealed class Game : PrototypeGame
{
	public static Task Main() => StartAsync<Game>(new()
	{
		Platform = new DesktopPlatform
		{
			Configuration =
			{
				Title = "Hello, Surreal!",
				IsVsyncEnabled = true,
				ShowFpsInTitle = true
			}
		}
	});
}
