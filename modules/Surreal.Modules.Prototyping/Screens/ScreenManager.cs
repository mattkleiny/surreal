namespace Surreal.Screens;

public interface IScreen
{
  void OnUpdate(GameTime time);
}

public interface IScreenManager
{
  IScreen? ActiveScreen { get; }

  void PushScreen(IScreen screen);
  IScreen? PopScreen();
}

public sealed class ScreenManager : IScreenManager
{
  private readonly LinkedList<IScreen> screens = new();

  public IScreen? ActiveScreen => screens.Last?.Value;

  public void PushScreen(IScreen screen)
  {
    screens.AddLast(screen);
  }

  public IScreen? PopScreen()
  {
    if (screens.Last != null)
    {
      var result = screens.Last.Value;

      screens.RemoveLast();

      return result;
    }

    return default;
  }
}

public static class ScreenManagerExtensions
{
  public static Task ExecuteScreen<TScreen>(this Game game)
    where TScreen : class, IScreen
  {
    var manager = new ScreenManager();

    game.Services.AddSingleton(game);
    game.Services.AddSingleton<IScreenManager>(manager);

    manager.PushScreen(game.Services.Create<TScreen>());

    game.ExecuteVariableStep(time =>
    {
      manager.ActiveScreen?.OnUpdate(time);
    });

    return Task.CompletedTask;
  }
}
