using System.Threading.Tasks;
using Asteroids.Actors;
using Surreal;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Framework.Simulations;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Mathematics;
using Surreal.States;
using Surreal.Timing;

namespace Asteroids.Screens {
  public sealed class MainScreen : SimulationScreen<AsteroidsGame, ActorSimulation> {
    public MainScreen(AsteroidsGame game)
        : base(game) {
    }

    private EmbeddedTimer countdownTimer = new EmbeddedTimer(1.Seconds());

    private Texture?   shipSprite;
    private Texture[]? asteroidSprites;

    public FSM<States> State { get; } = new FSM<States>(States.Playing);

    public float    SpawnRadius   { get; set; } = 800f;
    public IntRange AsteroidRange { get; set; } = Range.Of(32, 128);

    protected override ActorSimulation CreateSimulation() {
      return new ActorSimulation();
    }

    public override void Initialize() {
      base.Initialize();

      Restart(new Seed("LLAMAS"));
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

      Simulation.Scene.Actors.Clear();
      Simulation.Scene.Actors.Add(new Ship(shipSprite!));

      for (var i = 0; i < random.NextRange(AsteroidRange); i++) {
        Simulation.Scene.Actors.Add(new Asteroid(asteroidSprites!.SelectRandomly(random)) {
            Position = random.NextUnitCircle() * SpawnRadius,
        });
      }
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();
      if (Keyboard.IsKeyPressed(Key.Space)) Restart();

      base.Input(time);
    }

    public override void Update(GameTime time) {
      base.Update(time);

      if (State == States.Starting) {
        if (countdownTimer.Tick(time.DeltaTime)) {
          State.ChangeState(States.Playing);
          Restart();

          countdownTimer.Reset();
        }
      }
    }

    public enum States {
      Starting,
      Playing,
      GameOver
    }
  }
}