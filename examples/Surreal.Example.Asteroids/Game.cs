using Asteroids.Actors;
using Surreal;
using Surreal.Framework;
using Surreal.Graphics.Sprites;
using Surreal.Input.Keyboard;
using Surreal.Platform;

namespace Asteroids
{
  public sealed class Game : GameJam<Game>
  {
    public static void Main() => Start<Game>(new()
    {
      Platform = new DesktopPlatform
      {
        Configuration =
        {
          Title          = "Asteroids",
          IsVsyncEnabled = true,
          ShowFPSInTitle = true,
        },
      },
    });

    public ActorScene Scene { get; } = new();

    protected override void Initialize()
    {
      base.Initialize();

      var sprite = new Sprite(GraphicsDevice.CreateTexture());

      Scene.Spawn(new PlayerShip(Scene, sprite));
    }

    protected override void Input(GameTime time)
    {
      if (Keyboard.IsKeyPressed(Key.Escape))
      {
        Exit();
      }

      base.Input(time);

      Scene.Input(time);
    }

    protected override void Update(GameTime time)
    {
      base.Update(time);

      Scene.Update(time);
    }

    protected override void Draw(GameTime time)
    {
      base.Draw(time);

      Scene.Draw(time);
    }
  }
}