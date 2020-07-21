using System.Numerics;
using Isaac.Core.Items;
using Surreal.Framework.Parameters;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Mobs {
  public sealed class Player : Mob {
    public override Parameter<Vector2> Position  => State.Player.Position;
    public override Parameter<float>   Speed     => State.Player.Speed;
    public override Parameter<int>     Health    => State.Player.Health;
    public          Parameter<int>     Coins     => State.Player.Coins;
    public          Inventory          Inventory => State.Player.Inventory;

    public Player(TextureRegion sprite)
        : base(sprite) {
    }

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