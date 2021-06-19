using System.Numerics;
using Surreal.Framework;
using Surreal.Graphics.Sprites;
using Surreal.Input.Keyboard;

namespace Asteroids.Actors
{
  public sealed class PlayerShip : SpriteActor
  {
    public PlayerShip(IActorContext context, Sprite sprite)
        : base(context, sprite)
    {
    }

    protected override void OnInput(GameTime time)
    {
      var keyboard = Game.Current.Keyboard;

      if (keyboard.IsKeyDown(Key.W)) Position += new Vector2(0f, 1f);
      if (keyboard.IsKeyDown(Key.S)) Position += new Vector2(0f, -1f);
      if (keyboard.IsKeyDown(Key.A)) Position += new Vector2(1f, 0f);
      if (keyboard.IsKeyDown(Key.D)) Position += new Vector2(-1f, 0f);
    }
  }
}