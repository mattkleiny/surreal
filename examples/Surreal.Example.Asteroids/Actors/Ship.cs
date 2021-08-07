using System.Numerics;
using Surreal.Framework;
using Surreal.Input.Keyboard;

namespace Asteroids.Actors
{
  public class Ship : Actor
  {
    private readonly IKeyboardDevice keyboard;

    public Ship(IKeyboardDevice keyboard)
    {
      this.keyboard = keyboard;
    }

    protected override void OnInput(GameTime time)
    {
      ref var transform = ref Transform;

      if (keyboard.IsKeyDown(Key.W)) transform.Position += new Vector2(0f, 1f);
      if (keyboard.IsKeyDown(Key.S)) transform.Position += new Vector2(0f, -1f);
      if (keyboard.IsKeyDown(Key.A)) transform.Position += new Vector2(1f, 0f);
      if (keyboard.IsKeyDown(Key.D)) transform.Position += new Vector2(-1f, 0f);
    }
  }
}
