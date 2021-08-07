using Asteroids.Actors;
using Surreal.Assets;
using Surreal.Fibers;
using Surreal.Framework;
using Surreal.Framework.Actors;
using Surreal.Framework.Screens;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Mathematics;

namespace Asteroids.Screens
{
  public sealed class MainScreen : Screen<Game>
  {
    private Asset<Texture>    shipSprite;
    private Asset<Texture>[]? asteroidSprites;

    public MainScreen(Game game)
        : base(game)
    {
    }

    public ActorScene Scene { get; } = new();

    public override void Initialize()
    {
      base.Initialize();

      Restart(Seed.Randomized);
    }

    protected override FiberTask LoadContentAsync(IAssetResolver assets)
    {
      shipSprite = assets.LoadAsset<Texture>("Assets/sprites/ship.png");

      asteroidSprites = new[]
      {
        assets.LoadAsset<Texture>("Assets/sprites/large_asteroid_1.png"),
        assets.LoadAsset<Texture>("Assets/sprites/large_asteroid_2.png"),
        assets.LoadAsset<Texture>("Assets/sprites/large_asteroid_3.png"),
        assets.LoadAsset<Texture>("Assets/sprites/medium_asteroid_1.png"),
        assets.LoadAsset<Texture>("Assets/sprites/medium_asteroid_2.png"),
        assets.LoadAsset<Texture>("Assets/sprites/medium_asteroid_3.png"),
        assets.LoadAsset<Texture>("Assets/sprites/small_asteroid_1.png"),
        assets.LoadAsset<Texture>("Assets/sprites/small_asteroid_2.png"),
        assets.LoadAsset<Texture>("Assets/sprites/small_asteroid_3.png"),
      };

      return FiberTask.CompletedTask;
    }

    private void Restart(Seed seed = default)
    {
      Scene.Clear();
      Scene.Spawn(new Ship());
    }

    public override void Input(GameTime time)
    {
      if (Game.Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();
      if (Game.Keyboard.IsKeyPressed(Key.Space)) Restart();

      base.Input(time);
    }
  }
}
