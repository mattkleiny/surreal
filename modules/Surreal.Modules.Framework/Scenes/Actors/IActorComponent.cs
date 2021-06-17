using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Scenes.Actors {
  public interface IActorComponent {
    Actor? Actor { get; set; }

    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
  }
}