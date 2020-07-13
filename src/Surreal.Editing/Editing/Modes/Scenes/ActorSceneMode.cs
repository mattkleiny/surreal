using Surreal.Framework.Scenes.Actors;

namespace Surreal.Framework.Editing.Modes.Scenes {
  public abstract class ActorSceneMode : SceneMode {
    public ActorScene Scene { get; }

    public ActorSceneMode(ActorScene scene) {
      Scene = scene;
    }
  }
}