using System.Threading.Tasks;
using Asteroids.Actors;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Framework;
using Surreal.Framework.Scenes;
using Surreal.Framework.Scenes.Actors;
using Surreal.Framework.Screens;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Mathematics;

namespace Asteroids.Screens {
  public sealed class MainScreen : GameScreen<Game> {
    private Texture?   shipSprite;
    private Texture[]? asteroidSprites;

    public MainScreen(Game game)
        : base(game) {
    }

    public ActorScene Scene         { get; }      = new();
    public float      SpawnRadius   { get; set; } = 800f;
    public IntRange   AsteroidRange { get; set; } = Range.Of(32, 128);

    public override void Initialize() {
      base.Initialize();

      Plugins.Add(new ScenePlugin(Scene));

      Restart(Seed.Randomized);
    }

    protected override async Task LoadContentAsync(IAssetResolver assets) {
      await base.LoadContentAsync(assets);

      shipSprite = await assets.GetAsync<Texture>("Assets/sprites/ship.png");

      asteroidSprites = await Task.WhenAll(
          assets.GetAsync<Texture>("Assets/sprites/large_asteroid_1.png"),
          assets.GetAsync<Texture>("Assets/sprites/large_asteroid_2.png"),
          assets.GetAsync<Texture>("Assets/sprites/large_asteroid_3.png"),
          assets.GetAsync<Texture>("Assets/sprites/medium_asteroid_1.png"),
          assets.GetAsync<Texture>("Assets/sprites/medium_asteroid_2.png"),
          assets.GetAsync<Texture>("Assets/sprites/medium_asteroid_3.png"),
          assets.GetAsync<Texture>("Assets/sprites/small_asteroid_1.png"),
          assets.GetAsync<Texture>("Assets/sprites/small_asteroid_2.png"),
          assets.GetAsync<Texture>("Assets/sprites/small_asteroid_3.png")
      );
    }

    private void Restart(Seed seed = default) {
      var random = seed.ToRandom();

      Scene.Actors.Clear();
      Scene.Actors.Add(new Player(shipSprite!.ToRegion()));

      for (var i = 0; i < random.NextRange(AsteroidRange); i++) {
        Scene.Actors.Add(new Asteroid(asteroidSprites!.SelectRandomly(random).ToRegion()) {
            Position = random.NextUnitCircle() * SpawnRadius,
        });
      }
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();
      if (Keyboard.IsKeyPressed(Key.Space)) Restart();

      base.Input(time);
    }
  }
}