using System.Numerics;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Mathematics;
using Surreal.Mathematics.Timing;
using Surreal.States;

namespace Asteroids.Actors {
  public sealed class Player : Actor {
    private Vector2 direction = Vector2.Zero;
    private Vector2 facing    = Vector2.Zero;

    public Player(TextureRegion sprite)
        : base(sprite) {
    }

    public FSM<States> State  { get; }      = new(States.Alive);
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

      if (direction != Vector2.Zero) {
        facing = direction;
      }
    }

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      Position += direction * Speed * deltaTime;
      Rotation =  Angle.Between(Vector2.UnitY, facing);
    }

    public enum States {
      Alive,
      Dead
    }
  }
}