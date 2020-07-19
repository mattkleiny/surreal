using System.Numerics;
using System.Threading.Tasks;
using Prelude.Core;
using Prelude.Core.Actors;
using Prelude.Graphics;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Framework;
using Surreal.Framework.Scenes.Actors;
using Surreal.Framework.Screens;
using Surreal.Framework.Tiles;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;

namespace Prelude.Screens {
  public class MainScreen : GameScreen<Game> {
    private readonly ActorScene scene  = new ActorScene();
    private readonly Camera     camera = new Camera();

    private TileMapRenderer?    renderer;
    private Atlas<ImageRegion>? atlas;
    private TileMap<Tile>?      map;

    public MainScreen(Game game)
        : base(game) {
    }

    public override void Initialize() {
      base.Initialize();

      renderer = new TileMapRenderer(camera, atlas!);

      scene.Actors.Add(new Player(map!, camera) {
          Position  = new Vector2(2, 2),
          Direction = new Vector2(0, 1)
      });
    }

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
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

      base.Input(time);

      scene.Input(time.DeltaTime);
    }

    public override void Update(GameTime time) {
      base.Update(time);

      scene.Update(time.DeltaTime);
    }

    public override void Draw(GameTime time) {
      renderer?.Render(SpriteBatch, map!);

      base.Draw(time);

      scene.Draw(time.DeltaTime);
    }

    public override void Dispose() {
      base.Dispose();

      renderer?.Dispose();
    }
  }
}