using Surreal.Assets;
using Surreal.Collections;

namespace Surreal.Screens;

/// <summary>A screen in the <see cref="IScreenManager"/>.</summary>
public interface IScreen : IInterlinkedElement<IScreen>, IDisposable
{
  bool IsInitialized { get; }
  bool IsDisposed    { get; }

  ValueTask InitializeAsync();

  void Show();
  void Hide();

  void Input(GameTime time);
  void Update(GameTime time);
  void Draw(GameTime time);
}

/// <summary>Base class for any <see cref="IScreen"/> implementation.</summary>
public abstract class Screen : IScreen
{
  protected Screen(Game game)
  {
    Game = game;
  }

  public Game Game { get; }

  public bool IsInitialized { get; private set; }
  public bool IsDisposed    { get; private set; }

  public virtual async ValueTask InitializeAsync()
  {
    IsInitialized = true;

    await LoadContentAsync(Game.Assets);
  }

  protected virtual ValueTask LoadContentAsync(IAssetManager assets)
  {
    return ValueTask.CompletedTask;
  }

  public virtual void Show()
  {
  }

  public virtual void Hide()
  {
  }

  public virtual void Input(GameTime time)
  {
  }

  public virtual void Update(GameTime time)
  {
  }

  public virtual void Draw(GameTime time)
  {
  }

  public virtual void Dispose()
  {
    IsDisposed = true;
  }

  IScreen? IInterlinkedElement<IScreen>.Previous { get; set; }
  IScreen? IInterlinkedElement<IScreen>.Next     { get; set; }
}

/// <summary>Base class for any <see cref="IScreen"/> implementation.</summary>
public abstract class Screen<TGame> : Screen
  where TGame : Game
{
  protected Screen(TGame game)
    : base(game)
  {
  }

  public new TGame Game => (TGame) base.Game;
}
