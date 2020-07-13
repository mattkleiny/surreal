using Surreal.Framework.Scenes.Entities;

namespace Surreal.Framework.Editing.Modes.Scenes {
  public abstract class EntitySceneMode : SceneMode {
    public EntityScene Scene { get; }

    public EntitySceneMode(EntityScene scene) {
      Scene = scene;
    }
  }
}