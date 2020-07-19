using System.Numerics;
using Isaac.Core.Items;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Mobs {
  public sealed class Player : Mob {
    public Inventory Inventory { get; } = new Inventory();

    public override void Input(DeltaTime deltaTime) {
      base.Input(deltaTime);

      var keyboard  = Game.Current.Keyboard;
      var direction = Vector2.Zero;

      if (keyboard.IsKeyDown(Key.W)) direction.Y += 1;
      if (keyboard.IsKeyDown(Key.S)) direction.Y -= 1;
      if (keyboard.IsKeyDown(Key.A)) direction.X -= 1;
      if (keyboard.IsKeyDown(Key.D)) direction.X += 1;

      Direction.Value = direction;
    }
  }
}