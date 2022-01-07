using Surreal.Assets;
using Surreal.Collections;

namespace Surreal;

/// <summary>A plugin for a <see cref="Game"/>.</summary>
public interface IGamePlugin : IDisposable
{
  Task LoadContentAsync(IAssetManager assets, CancellationToken cancellationToken = default);
  Task InitializeAsync(CancellationToken cancellationToken = default);

  void Input(GameTime time);
  void Update(GameTime time);
  void Draw(GameTime time);
}

/// <summary>A registry for <see cref="IGamePlugin"/>s.</summary>
public interface IGamePluginRegistry
{
  ReadOnlySlice<IGamePlugin> ActivePlugins { get; }

  void Add(IGamePlugin plugin);
  void Remove(IGamePlugin plugin);

  void Clear();
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

  public virtual Task LoadContentAsync(IAssetManager assets, CancellationToken cancellationToken = default)
  {
    return Task.CompletedTask;
  }

  public virtual Task InitializeAsync(CancellationToken cancellationToken = default)
  {
    return Task.CompletedTask;
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
