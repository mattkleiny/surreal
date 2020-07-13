using System.Numerics;
using Surreal.Framework.Scenes.Actors;
using Surreal.Input.Keyboard;
using Surreal.Timing;

namespace Asteroids.Actors.Components {
  public sealed class InputComponent : ActorComponent<AsteroidActor> {
    private Vector2 direction;

    public InputComponent(AsteroidActor actor)
        : base(actor) {
    }

    public override void Input(DeltaTime deltaTime) {
      base.Input(deltaTime);

      var direction = Vector2.Zero;

      if (AsteroidsGame.Current.Keyboard.IsKeyPressed(Key.W)) direction.Y += 1f;
      if (AsteroidsGame.Current.Keyboard.IsKeyPressed(Key.S)) direction.Y -= 1f;
      if (AsteroidsGame.Current.Keyboard.IsKeyPressed(Key.A)) direction.X -= 1f;
      if (AsteroidsGame.Current.Keyboard.IsKeyPressed(Key.D)) direction.X += 1f;

      this.direction = Vector2.Normalize(direction);
    }

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      Actor.Position += direction * deltaTime;

      direction = Vector2.Zero;
    }
  }
}