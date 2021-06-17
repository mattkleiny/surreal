using System.Numerics;
using Prelude.Core;
using Prelude.Screens;
using Surreal;
using Surreal.Assets;
using Surreal.Framework;
using Surreal.Framework.Tiles;
using Surreal.Platform;

namespace Prelude {
  public sealed class Game : GameJam<Game> {
    private static readonly Matrix4x4 ProjectionView = Matrix4x4.CreateOrthographic(1920f, 1080f, 0.1f, 300f);

    public static void Main() => Start<Game>(new Configuration {
      Platform = new DesktopPlatform {
        Configuration = {
          Title          = "Prelude of the Chambered",
          IsVsyncEnabled = true,
          ShowFPSInTitle = true,
        },
      },
    });

    protected override void RegisterAssetLoaders(AssetManager assets) {
      base.RegisterAssetLoaders(assets);

      assets.RegisterLoader(new TileMap<Tile>.Loader(Tile.Palette));
    }

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));
    }

    protected override void Begin(GameTime time) {
      base.Begin(time);

      SpriteBatch.Begin(in ProjectionView);
    }

    protected override void End(GameTime time) {
      SpriteBatch.End();

      base.End(time);
    }
  }
}