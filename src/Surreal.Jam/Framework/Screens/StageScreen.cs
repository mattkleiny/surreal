using Surreal.Framework.Screens.Plugins;
using Surreal.UI;

namespace Surreal.Framework.Screens {
  public abstract class StageScreen<TGame> : GameScreen<TGame>
      where TGame : GameJam {
    protected StageScreen(TGame game)
        : base(game) {
    }

    public Stage Stage { get; private set; }

    protected abstract Stage CreateStage();

    public override void Initialize() {
      base.Initialize();

      Stage = CreateStage();

      Plugins.Add(new StagePlugin(Stage, Game.SpriteBatch));
    }
  }
}