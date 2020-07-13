using System;
using Surreal.Graphics.Sprites;
using Surreal.UI;

namespace Surreal.Framework.Screens.Plugins {
  public class StagePlugin : ScreenPlugin, IDisposable {
    public Stage       Stage       { get; }
    public SpriteBatch SpriteBatch { get; }

    public StagePlugin(Stage stage, SpriteBatch batch) {
      Stage       = stage;
      SpriteBatch = batch;
    }

    public override void Begin() {
      Stage.Begin();

      base.Begin();
    }

    public override void Input(GameTime time) {
      base.Input(time);

      Stage.Input(time.DeltaTime);
    }

    public override void Update(GameTime time) {
      base.Update(time);

      Stage.Update(time.DeltaTime);
    }

    public override void Draw(GameTime time) {
      base.Draw(time);

      Stage.Draw(time.DeltaTime, SpriteBatch);
    }

    public override void End() {
      base.End();

      Stage.End();
    }

    public virtual void Dispose() {
      Stage.Dispose();
    }
  }
}