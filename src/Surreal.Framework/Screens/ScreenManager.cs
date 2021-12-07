using Surreal.Collections;

namespace Surreal.Screens;

/// <summary>A manager for <see cref="IScreen"/>s.</summary>
public interface IScreenManager : IGamePlugin
{
  event Action<IScreen?> ScreenChanged;

  IScreen? ActiveScreen   { get; }
  IScreen? PreviousScreen { get; }

  void     Push(IScreen screen);
  void     Replace(IScreen screen);
  IScreen? Pop(bool dispose = true);
}

/// <summary>The default <see cref="IScreenManager"/> implementation.</summary>
public sealed class ScreenManager : GamePlugin<Game>, IScreenManager
{
  private readonly LinkedNodeList<IScreen> screens = new();

  public ScreenManager(Game game)
    : base(game)
  {
  }

  public event Action<IScreen?>? ScreenChanged;

  public IScreen? ActiveScreen   => screens.Head;
  public IScreen? PreviousScreen => screens.Head?.Previous;

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
    screens.Add(screen);

    ScreenChanged?.Invoke(ActiveScreen);
  }

  public void Replace(IScreen screen)
  {
    Pop();
    Push(screen);
  }

  public IScreen? Pop(bool dispose = true)
  {
    if (screens.IsEmpty) return null;

    var screen = screens.Head!;

    screen.Hide();
    screens.Remove(screen);

    if (dispose)
    {
      screen.Dispose();
    }

    ScreenChanged?.Invoke(ActiveScreen);

    return screen;
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
    while (!screens.IsEmpty)
    {
      Pop();
    }

    base.Dispose();
  }
}