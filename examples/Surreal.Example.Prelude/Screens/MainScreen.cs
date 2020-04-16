using System.Numerics;
using System.Threading.Tasks;
using Prelude.Core;
using Prelude.Core.Actors;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Framework.Simulations;
using Surreal.Framework.Tiles;
using Surreal.Graphics.Raycasting;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;

namespace Prelude.Screens
{
  public class MainScreen : SimulationScreen<PreludeGame, ActorSimulation>
  {
    private readonly RaycastCamera camera = new RaycastCamera();

    private TileMapRenderer?     renderer;
    private Atlas<PixmapRegion>? atlas;
    private TileMap<Tile>?       map;

    public MainScreen(PreludeGame game)
      : base(game)
    {
    }

    protected override ActorSimulation CreateSimulation()
    {
      return new ActorSimulation();
    }

    public override void Initialize()
    {
      base.Initialize();

      renderer = new TileMapRenderer(camera, atlas!);

      Simulation.Scene.Actors.Add(new PreludeActor()
      {
        new Player(map!, camera)
        {
          Position  = new Vector2(2, 2),
          Direction = new Vector2(0, 1)
        }
      });
    }

    protected override async Task LoadContentAsync(IAssetResolver assets)
    {
      await base.LoadContentAsync(assets);

      atlas = Atlas<PixmapRegion>.Create(
        source: await assets.GetAsync<Pixmap>("Assets/textures.png"),
        nameTemplate: "textures_",
        regionWidth: 16,
        regionHeight: 16
      );

      map = await assets.GetAsync<TileMap<Tile>>("Assets/maps/map1.tmx");
    }

    public override void Input(GameTime time)
    {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

      base.Input(time);
    }

    public override void Draw(GameTime time)
    {
      renderer?.Render(GraphicsDevice, SpriteBatch, map!);

      base.Draw(time);
    }

    public override void Dispose()
    {
      base.Dispose();

      renderer?.Dispose();
    }
  }
}
