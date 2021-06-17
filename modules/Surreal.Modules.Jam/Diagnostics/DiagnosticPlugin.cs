using Surreal.Framework;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Linear;
using Surreal.UI;

namespace Surreal.Diagnostics {
  public abstract class DiagnosticPlugin<TGame> : GamePlugin<TGame>
      where TGame : GameJam {
    protected DiagnosticPlugin(TGame game)
        : base(game) {
      Stage = new Stage(
          viewport: StageViewports.Fixed(Size.X, Size.Y),
          layout: new Rectangle(0f, 0f, Size.X, Size.Y)
      );
    }

    public Vector2I Size      { get; set; } = new(640, 480);
    public Stage    Stage     { get; set; }
    public bool     IsVisible { set; get; }

    public IKeyboardDevice Keyboard => Game.Keyboard;

    public override void Input(GameTime time) {
      base.Input(time);

      if (IsVisible) {
        Stage.Input(time.DeltaTime);
      }
    }

    public override void Update(GameTime time) {
      base.Update(time);

      if (IsVisible) {
        Stage.Update(time.DeltaTime);
      }
    }

    public override void Draw(GameTime time) {
      base.Draw(time);

      if (IsVisible) {
        Stage.Draw(time.DeltaTime, Game.SpriteBatch);
      }
    }

    public override void Dispose() {
      Stage.Dispose();

      base.Dispose();
    }
  }
}