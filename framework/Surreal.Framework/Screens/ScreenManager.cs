using System;
using System.Collections.Generic;

namespace Surreal.Framework.Screens
{
  public interface IScreenManager : IGamePlugin
  {
    event Action<IScreen?> ScreenChanged;

    IScreen? ActiveScreen   { get; }
    IScreen? PreviousScreen { get; }

    void     Push(IScreen screen);
    void     Replace(IScreen screen);
    IScreen? Pop(bool dispose = true);
  }

  public sealed class ScreenManager : GamePlugin<Game>, IScreenManager
  {
    private readonly LinkedList<IScreen> screens = new();

    public ScreenManager(Game game)
        : base(game)
    {
    }

    public event Action<IScreen?>? ScreenChanged;

    public IScreen? ActiveScreen   => screens.Last?.Value;
    public IScreen? PreviousScreen => screens.Last?.Previous?.Value;

    public override void Initialize()
    {
      ActiveScreen?.Initialize();
    }

    public void Push(IScreen screen)
    {
      ActiveScreen?.Hide();

      if (!screen.IsInitialized)
      {
        screen.Initialize();
      }

      screen.Show();
      screens.AddLast(screen);

      ScreenChanged?.Invoke(ActiveScreen);
    }

    public void Replace(IScreen screen)
    {
      Pop();
      Push(screen);
    }

    public IScreen? Pop(bool dispose = true)
    {
      if (screens.Count > 0)
      {
        var node   = screens.Last!;
        var screen = node.Value;

        screens.Remove(node);
        node.Value.Hide();

        if (dispose)
        {
          screen.Dispose();
        }

        ScreenChanged?.Invoke(ActiveScreen);

        return screen;
      }

      return null;
    }

    public override void Input(GameTime time)
    {
      ActiveScreen?.Input(new GameTime(
          deltaTime: time.DeltaTime,
          totalTime: time.TotalTime,
          isRunningSlowly: time.IsRunningSlowly
      ));
    }

    public override void Update(GameTime time)
    {
      ActiveScreen?.Update(new GameTime(
          deltaTime: time.DeltaTime,
          totalTime: time.TotalTime,
          isRunningSlowly: time.IsRunningSlowly
      ));
    }

    public override void Draw(GameTime time)
    {
      ActiveScreen?.Draw(new GameTime(
          deltaTime: time.DeltaTime,
          totalTime: time.TotalTime,
          isRunningSlowly: time.IsRunningSlowly
      ));
    }

    public override void Dispose()
    {
      while (screens.Count > 0)
      {
        Pop();
      }

      base.Dispose();
    }
  }
}