using Surreal.Timing;

namespace Surreal.Framework.Scenes.Actors {
  public interface IActorComponent {
    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
  }

  public abstract class ActorComponent<TActor> : IActorComponent
      where TActor : Actor {
    public TActor Actor { get; }

    protected ActorComponent(TActor actor) {
      Actor = actor;
    }

    public virtual void Input(DeltaTime deltaTime) {
    }

    public virtual void Update(DeltaTime deltaTime) {
    }

    public virtual void Draw(DeltaTime deltaTime) {
    }
  }
}