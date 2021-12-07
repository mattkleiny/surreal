using Surreal.Assets;
using Surreal.Fibers;

namespace Surreal;

/// <summary>A plugin for a <see cref="Game"/>.</summary>
public interface IGamePlugin : IDisposable
{
  FiberTask LoadContentAsync(IAssetResolver assets);
  void      Initialize();

  void Input(GameTime time);
  void Update(GameTime time);
  void Draw(GameTime time);
}

/// <summary>Base class for any <see cref="IGamePlugin"/> implementation.</summary>
public abstract class GamePlugin : GamePlugin<Game>
{
  protected GamePlugin(Game game)
    : base(game)
  {
  }
}

/// <summary>Base class for any <see cref="IGamePlugin"/> implementation.</summary>
public abstract class GamePlugin<TGame> : IGamePlugin
  where TGame : Game
{
  protected GamePlugin(TGame game)
  {
    Game = game;
  }

  public TGame Game { get; }

  public virtual FiberTask LoadContentAsync(IAssetResolver assets)
  {
    return FiberTask.CompletedTask;
  }

  public virtual void Initialize()
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
  }
}