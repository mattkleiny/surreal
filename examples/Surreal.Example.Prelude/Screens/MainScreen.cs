using System.Numerics;
using System.Threading.Tasks;
using Prelude.Core;
using Prelude.Core.Actors;
using Prelude.Graphics;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Framework;
using Surreal.Framework.Scenes;
using Surreal.Framework.Scenes.Actors;
using Surreal.Framework.Screens;
using Surreal.Framework.Tiles;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;

namespace Prelude.Screens {
  public class MainScreen : GameScreen<Game> {
    private TileMapRenderer?    renderer;
    private Atlas<ImageRegion>? atlas;
    private TileMap<Tile>?      map;

    public MainScreen(Game game)
        : base(game) {
    }

    public ActorScene Scene  { get; } = new ActorScene();
    public Camera     Camera { get; } = new Camera();

    protected override async Task LoadContentAsync(IAssetResolver assets) {
      await base.LoadContentAsync(assets);

      var image = await assets.GetAsync<Image>("Assets/textures.png");

      atlas = Atlas<ImageRegion>.Create(
          source: image.ToRegion(),
          nameTemplate: "textures_",
          regionWidth: 16,
          regionHeight: 16
      );

      map = await assets.GetAsync<TileMap<Tile>>("Assets/maps/map1.tmx");

      renderer = new TileMapRenderer(Camera, atlas!);
    }

    public override void Initialize() {
      base.Initialize();

      Plugins.Add(new ScenePlugin(Scene));

      Scene.Actors.Add(new Player(map!, Camera) {
          Position  = new Vector2(2, 2),
          Direction = new Vector2(0, 1)
      });
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

      base.Input(time);
    }

    public override void Draw(GameTime time) {
      renderer?.Render(SpriteBatch, map!);

      base.Draw(time);
    }

    public override void Dispose() {
      base.Dispose();

      renderer?.Dispose();
    }
  }
}