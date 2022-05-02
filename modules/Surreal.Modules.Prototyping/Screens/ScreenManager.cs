using System.Reflection;

namespace Surreal.Screens;

public interface IScreen
{
  void OnUpdate(GameTime time, Game game);
  void OnRender(GameTime time, Game game);
}

public interface IScreenManager
{
  IScreen? ActiveScreen { get; }
}

public sealed class ScreenManager : IScreenManager
{
  private readonly LinkedList<IScreen> screens = new();

  public IScreen? ActiveScreen => screens.Last?.Value;

  public void Push(IScreen screen)
  {
    screens.AddLast(screen);
  }

  public void Pop()
  {
    if (screens.Last != null)
    {
      screens.RemoveLast();
    }
  }
}

public static class ScreenManagerExtensions
{
  public static Task ExecuteScreen<TScreen>(this Game game)
    where TScreen : class, IScreen
  {
    var manager = new ScreenManager();

    game.Services.AddSingleton<IScreenManager>(manager);

    manager.Push(game.Services.Create<TScreen>());

    game.ExecuteVariableStep(time =>
    {
      var activeScreen = manager.ActiveScreen;

      activeScreen?.OnUpdate(time, game);
      activeScreen?.OnRender(time, game);
    });

    return Task.CompletedTask;
  }
}
