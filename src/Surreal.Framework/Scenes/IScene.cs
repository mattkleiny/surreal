using Surreal.Timing;

namespace Surreal.Framework.Scenes {
  public interface IScene {
    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
  }
}