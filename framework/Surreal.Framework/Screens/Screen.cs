using System;
using Surreal.Assets;
using Surreal.Fibers;

namespace Surreal.Framework.Screens
{
  public interface IScreen : IDisposable
  {
    bool IsInitialized { get; }
    bool IsDisposed    { get; }

    void Initialize();

    void Show();
    void Hide();

    void Input(GameTime time);
    void Update(GameTime time);
    void Draw(GameTime time);
  }

  public abstract class Screen : IScreen
  {
    protected Screen(Game game)
    {
      Game = game;
    }

    public Game Game { get; }

    public bool IsInitialized { get; private set; }
    public bool IsDisposed    { get; private set; }

    public virtual void Initialize()
    {
      IsInitialized = true;

      LoadContentAsync(Game.Assets).Forget();
    }

    protected virtual FiberTask LoadContentAsync(IAssetResolver assets)
    {
      return FiberTask.CompletedTask;
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
  }

  public abstract class Screen<TGame> : Screen
      where TGame : Game
  {
    protected Screen(TGame game)
        : base(game)
    {
    }

    public new TGame Game => (TGame) base.Game;
  }
}