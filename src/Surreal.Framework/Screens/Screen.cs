using System.Collections.Generic;
using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Screens {
  public abstract class Screen : IScreen {
    protected Screen(Game game) {
      Game  = game;
      Clock = Clocks.Relative(game.Clock);
    }

    public Game                Game    { get; }
    public IClock              Clock   { get; }
    public List<IScreenPlugin> Plugins { get; } = new();

    public bool IsInitialized { get; private set; }
    public bool IsDisposed    { get; private set; }

    public virtual void Initialize() {
      IsInitialized = true;
    }

    public virtual void Show() {
      for (var i = 0; i < Plugins.Count; i++) {
        Plugins[i].Show();
      }
    }

    public virtual void Hide() {
      for (var i = 0; i < Plugins.Count; i++) {
        Plugins[i].Hide();
      }
    }

    public virtual void Input(GameTime time) {
      for (var i = 0; i < Plugins.Count; i++) {
        Plugins[i].Input(time);
      }
    }

    public virtual void Update(GameTime time) {
      for (var i = 0; i < Plugins.Count; i++) {
        Plugins[i].Update(time);
      }
    }

    public virtual void Draw(GameTime time) {
      for (var i = 0; i < Plugins.Count; i++) {
        Plugins[i].Draw(time);
      }
    }

    public virtual void Dispose() {
      if (!IsDisposed) {
        for (var i = 0; i < Plugins.Count; i++) {
          Plugins[i].Dispose();
        }
      }

      IsDisposed = true;
    }
  }
}