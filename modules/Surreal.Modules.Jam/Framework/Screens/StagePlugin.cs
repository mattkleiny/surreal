using Surreal.Graphics.Sprites;
using Surreal.UI;

namespace Surreal.Framework.Screens {
  public sealed class StagePlugin : IScreenPlugin {
    public StagePlugin(Stage stage, SpriteBatch batch) {
      Stage = stage;
      Batch = batch;
    }

    public Stage       Stage { get; }
    public SpriteBatch Batch { get; }

    public void Show() {
    }

    public void Hide() {
    }

    public void Input(GameTime time)  => Stage.Input(time.DeltaTime);
    public void Update(GameTime time) => Stage.Update(time.DeltaTime);
    public void Draw(GameTime time)   => Stage.Draw(time.DeltaTime, Batch);

    public void Dispose() => Stage.Dispose();
  }
}