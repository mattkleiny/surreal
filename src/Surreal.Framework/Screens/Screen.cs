using Surreal.Timing;

namespace Surreal.Framework.Screens {
  public abstract class Screen : IScreen {
    protected Screen(Game game) {
      Game  = game;
      Clock = new FixedStepClock(16.Milliseconds());
    }

    public Game   Game  { get; }
    public IClock Clock { get; protected set; }

    public bool IsInitialized { get; private set; }
    public bool IsDisposed    { get; private set; }

    public virtual void Initialize() {
      IsInitialized = true;
    }

    public virtual void Show() {
    }

    public virtual void Hide() {
    }

    public virtual void Input(GameTime time) {
    }

    public virtual void Update(GameTime time) {
    }

    public virtual void Draw(GameTime time) {
    }

    public virtual void Dispose() {
      IsDisposed = true;
    }
  }
}