using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Scenes.Actors {
  public abstract class ActorComponent : IActorComponent {
    public Actor? Actor { get; set; }

    public virtual void Input(DeltaTime deltaTime) {
    }

    public virtual void Update(DeltaTime deltaTime) {
    }

    public virtual void Draw(DeltaTime deltaTime) {
    }
  }

  public abstract class ActorComponent<TActor> : ActorComponent
      where TActor : Actor {
    public new TActor? Actor {
      get => base.Actor as TActor;
      set => base.Actor = value;
    }
  }
}