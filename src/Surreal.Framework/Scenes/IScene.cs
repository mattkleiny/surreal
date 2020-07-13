using Surreal.Timing;

namespace Surreal.Framework.Scenes {
  // TODO: allow scenes inside of scenes
  // TODO: maybe make this a scene graph with different 'drivers' inside of the graph (for entities, actors, etc).

  public interface IScene {
    void Begin();
    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
    void End();
  }
}