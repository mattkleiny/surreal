using Surreal.Graphics.Sprites;
using Surreal.Mathematics.Linear;
using Surreal.Timing;
using Surreal.UI.Controls;

namespace Surreal.UI {
  public class Stage : Control {
    private readonly Rectangle layout;

    public StageViewport Viewport { get; }

    public Stage(StageViewport viewport, Rectangle layout) {
      this.layout = layout;

      Viewport = viewport;
    }

    protected override Rectangle ComputeLayout() {
      return layout;
    }

    public override void Draw(DeltaTime deltaTime, SpriteBatch batch) {
      batch.Begin(Viewport());

      base.Draw(deltaTime, batch);

      batch.End();
    }
  }
}