using System.Numerics;
using System.Threading.Tasks;
using Isaac.Core;
using Isaac.Core.Entities;
using Isaac.Core.Entities.Components;
using Isaac.Core.Entities.Systems;
using Isaac.Core.Maps;
using Isaac.Graphics;
using Surreal;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Framework;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Framework.Scenes.Entities.Storage;
using Surreal.Framework.Scenes.Entities.Systems;
using Surreal.Framework.Screens;
using Surreal.Framework.Simulations;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;

namespace Isaac.Screens
{
  public sealed class MainScreen : SimulationScreen<IsaacGame, EntitySimulation>, ILoadableScreen
  {
    private readonly OrthographicCamera camera = new OrthographicCamera(256, 144);

    private RoomCatalogue?        roomCatalogue;
    private Atlas<TextureRegion>? textureAtlas;
    private Floor?                floor;

    public GameConfiguration Configuration { get; }

    public MainScreen(IsaacGame game, GameConfiguration configuration)
      : base(game)
    {
      Configuration = configuration;
    }

    protected override EntitySimulation CreateSimulation()
    {
      return new EntitySimulation();
    }

    public override void Initialize()
    {
      base.Initialize();

      CreateWorld();
      CreateMap();
      CreateEntities();
    }

    protected override async Task LoadContentAsync(IAssetResolver assets)
    {
      await base.LoadContentAsync(assets);

      textureAtlas = Atlas<TextureRegion>.Create(
        source: await assets.GetAsync<Texture>("Assets/tilesets/biome_desert.png"),
        nameTemplate: "textures_",
        regionWidth: 16,
        regionHeight: 16
      );
    }

    public async Task LoadInBackgroundAsync(IAssetResolver assets)
    {
      roomCatalogue = await assets.GetAsync<RoomCatalogue>("Assets/rooms/");
    }

    private void CreateWorld()
    {
      Simulation.Scene.RegisterComponent(new DenseComponentStorage<Transform>());
      Simulation.Scene.RegisterComponent(new DenseComponentStorage<RigidBody>());
      Simulation.Scene.RegisterComponent(new DenseComponentStorage<Sprite>());
      Simulation.Scene.RegisterComponent(new DenseComponentStorage<Health>());
      Simulation.Scene.RegisterComponent(new SparseComponentStorage<ParticleEmitter>());
      Simulation.Scene.RegisterComponent(new SparseComponentStorage<Player>());
      Simulation.Scene.RegisterComponent(new SparseComponentStorage<DamageOverTime>());

      Simulation.Scene.AddSystem(new PhysicsSystem
      {
        Gravity = Vector2.Zero,
      });

      Simulation.Scene.AddSystem(new CameraSystem(camera, Game.Host));
      Simulation.Scene.AddSystem(new SpriteSystem(SpriteBatch, camera));
      Simulation.Scene.AddSystem(new ParticleSystem(SpriteBatch, camera));
      Simulation.Scene.AddSystem(new InputSystem(camera, Keyboard, Mouse));
      Simulation.Scene.AddSystem(new CombatSystem(Simulation.Events));
    }

    private void CreateMap()
    {
      floor = FloorPlanner.Plan(
        seed: Configuration.Seed,
        catalogue: roomCatalogue!,
        floorType: FloorType.Basement,
        width: Configuration.FloorWidth,
        height: Configuration.FloorHeight
      );
    }

    private void CreateEntities()
    {
      Simulation.Scene.CreatePlayer(textureAtlas!["textures_11"]);

      for (var i = 0; i < 16; i++)
      {
        var enemy = Simulation.Scene.CreateEnemy(textureAtlas!["textures_11"]);

        enemy.Get<Transform>().Position = new Vector2(
          x: Maths.Random.Next(-camera.Viewport.Width / 2, camera.Viewport.Width / 2),
          y: Maths.Random.Next(-camera.Viewport.Height / 2, camera.Viewport.Height / 2)
        );
      }
    }

    public override void Input(GameTime time)
    {
      if (Keyboard.IsKeyPressed(Key.Escape))
      {
        Game.Exit();
      }

      base.Input(time);
    }

    public override void Draw(GameTime time)
    {
      floor?.Render(
        batch: SpriteBatch,
        textures: textureAtlas!,
        offset: new Vector2(
          x: -camera.Viewport.Width / 2f,
          y: -camera.Viewport.Height / 2f
        )
      );

      base.Draw(time);
    }
  }
}
