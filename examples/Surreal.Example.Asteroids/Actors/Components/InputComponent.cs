using System.Numerics;
using Surreal.Framework.Scenes.Actors;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Timing;

namespace Asteroids.Actors.Components {
  public sealed class InputComponent : ActorComponent<Actor> {
    private Vector2 direction;

    public float Speed { get; set; } = 100f;

    public override void Input(DeltaTime deltaTime) {
      base.Input(deltaTime);

      direction = Vector2.Zero;

      if (AsteroidsGame.Current.Keyboard.IsKeyDown(Key.W)) direction.Y += 1f;
      if (AsteroidsGame.Current.Keyboard.IsKeyDown(Key.S)) direction.Y -= 1f;
      if (AsteroidsGame.Current.Keyboard.IsKeyDown(Key.A)) direction.X -= 1f;
      if (AsteroidsGame.Current.Keyboard.IsKeyDown(Key.D)) direction.X += 1f;
    }

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      Actor!.Position += direction * Speed * deltaTime;
    }
  }
}