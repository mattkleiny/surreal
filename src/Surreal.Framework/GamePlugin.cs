using System;
using System.Threading.Tasks;
using Surreal.Assets;

namespace Surreal.Framework {
  public interface IGamePlugin : IDisposable {
    Task LoadContentAsync(IAssetResolver assets);
    void Initialize();

    void Input(GameTime time);
    void Update(GameTime time);
    void Draw(GameTime time);
  }

  public abstract class GamePlugin : GamePlugin<Game> {
    protected GamePlugin(Game game)
        : base(game) {
    }
  }

  public abstract class GamePlugin<TGame> : IGamePlugin
      where TGame : Game {
    protected GamePlugin(TGame game) {
      Game = game;
    }

    public TGame Game { get; }

    public virtual Task LoadContentAsync(IAssetResolver assets) {
      return Task.CompletedTask;
    }

    public virtual void Initialize() {
    }

    public virtual void Input(GameTime time) {
    }

    public virtual void Update(GameTime time) {
    }

    public virtual void Draw(GameTime time) {
    }

    public virtual void Dispose() {
    }
  }
}