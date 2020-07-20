using System.Numerics;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Timing;
using Surreal.States;

namespace Asteroids.Actors {
  public sealed class Ship : Actor {
    private Vector2 direction = Vector2.Zero;

    public Ship(TextureRegion sprite)
        : base(sprite) {
    }

    public FSM<States> State  { get; }      = new FSM<States>(States.Alive);
    public int         Health { get; set; } = 10;
    public int         Damage { get; set; } = 2;
    public float       Speed  { get; set; } = 100f;

    public override void Input(DeltaTime deltaTime) {
      base.Input(deltaTime);

      direction = Vector2.Zero;

      if (Game.Current.Keyboard.IsKeyDown(Key.W)) direction.Y += 1f;
      if (Game.Current.Keyboard.IsKeyDown(Key.S)) direction.Y -= 1f;
      if (Game.Current.Keyboard.IsKeyDown(Key.A)) direction.X -= 1f;
      if (Game.Current.Keyboard.IsKeyDown(Key.D)) direction.X += 1f;
    }

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      Position += direction * Speed * deltaTime;
    }

    public enum States {
      Alive,
      Dead
    }
  }
}