using System;
using Surreal.Framework.Screens;

namespace Surreal.Framework.Scenes {
  public sealed class ScenePlugin : IScreenPlugin {
    public ScenePlugin(IScene scene) {
      Scene = scene;
    }

    public IScene Scene { get; }

    public void Show() {
    }

    public void Hide() {
    }

    public void Input(GameTime time)  => Scene.Input(time.DeltaTime);
    public void Update(GameTime time) => Scene.Update(time.DeltaTime);
    public void Draw(GameTime time)   => Scene.Draw(time.DeltaTime);

    public void Dispose() {
      if (Scene is IDisposable disposable) {
        disposable.Dispose();
      }
    }
  }
}