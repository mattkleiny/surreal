using System;
using System.Collections.Generic;

namespace Surreal.Framework.Screens
{
  public sealed class ScreenManager : GamePlugin<Game>, IScreenManager
  {
    private readonly LinkedList<IScreen> screens = new LinkedList<IScreen>();

    public ScreenManager(Game game)
      : base(game)
    {
    }

    public event Action<IScreen?> ScreenChanged;

    public IScreen? ActiveScreen   => screens.Last?.Value;
    public IScreen? PreviousScreen => screens.Last?.Previous?.Value;

    public override void Initialize()
    {
      if (ActiveScreen != null)
      {
        ActiveScreen.Initialize();
      }
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

    public IScreen? Pop(bool dispose = true)
    {
      if (screens.Count > 0)
      {
        var screen = screens.Last.Value;
        screens.RemoveFirst();

        screen.Hide();
        if (dispose) screen.Dispose();

        ScreenChanged?.Invoke(ActiveScreen);

        return screen;
      }

      return null;
    }

    public override void Begin()
    {
      ActiveScreen?.Begin();
    }

    public override void Input(GameTime time)
    {
      ActiveScreen?.Input(new GameTime(
        deltaTime: ActiveScreen.Clock.DeltaTime,
        totalTime: time.TotalTime,
        isRunningSlowly: time.IsRunningSlowly
      ));
    }

    public override void Update(GameTime time)
    {
      ActiveScreen?.Update(new GameTime(
        deltaTime: ActiveScreen.Clock.DeltaTime,
        totalTime: time.TotalTime,
        isRunningSlowly: time.IsRunningSlowly
      ));
    }

    public override void Draw(GameTime time)
    {
      ActiveScreen?.Draw(new GameTime(
        deltaTime: ActiveScreen.Clock.DeltaTime,
        totalTime: time.TotalTime,
        isRunningSlowly: time.IsRunningSlowly
      ));
    }

    public override void End()
    {
      ActiveScreen?.End();
    }

    public override void Dispose()
    {
      while (screens.Count > 0) Pop();

      base.Dispose();
    }
  }
}