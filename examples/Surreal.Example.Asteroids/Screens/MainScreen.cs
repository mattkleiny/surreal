using Asteroids.Actors;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Screens;

namespace Asteroids.Screens;

public sealed class MainScreen : Screen<AsteroidsGame>
{
  private Texture?   shipSprite;
  private Texture[]? asteroidSprites;

  public MainScreen(AsteroidsGame game)
    : base(game)
  {
  }

  public float      SpawnRadius   { get; set; } = 800f;
  public IntRange   AsteroidRange { get; set; } = new(32, 128);
  public ActorScene Scene         { get; set; } = new();

  public override void Initialize()
  {
    base.Initialize();

    Restart(Seed.FromString("LLAMAS"));
  }

  public override async ValueTask LoadContentAsync(IAssetManager assets)
  {
    shipSprite = await assets.LoadAssetAsync<Texture>("Assets/sprites/ship.png");

    asteroidSprites = await Task.WhenAll(
      assets.LoadAssetAsync<Texture>("Assets/sprites/large_asteroid_1.png"),
      assets.LoadAssetAsync<Texture>("Assets/sprites/large_asteroid_2.png"),
      assets.LoadAssetAsync<Texture>("Assets/sprites/large_asteroid_3.png"),
      assets.LoadAssetAsync<Texture>("Assets/sprites/medium_asteroid_1.png"),
      assets.LoadAssetAsync<Texture>("Assets/sprites/medium_asteroid_2.png"),
      assets.LoadAssetAsync<Texture>("Assets/sprites/medium_asteroid_3.png"),
      assets.LoadAssetAsync<Texture>("Assets/sprites/small_asteroid_1.png"),
      assets.LoadAssetAsync<Texture>("Assets/sprites/small_asteroid_2.png"),
      assets.LoadAssetAsync<Texture>("Assets/sprites/small_asteroid_3.png")
    );
  }

  private void Restart(Seed seed = default)
  {
    var random = seed.ToRandom();

    Scene.Spawn(new Ship(Scene, shipSprite!));

    for (var i = 0; i < random.NextRange(AsteroidRange); i++)
    {
      Scene.Spawn(new Asteroid(Scene, asteroidSprites!.SelectRandomly(random)!));
    }
  }

  public override void Input(GameTime time)
  {
    if (Game.Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();
    if (Game.Keyboard.IsKeyPressed(Key.Space)) Restart();

    base.Input(time);
  }
}
