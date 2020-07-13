using System.Numerics;
using Prelude.Core;
using Prelude.Screens;
using Surreal;
using Surreal.Assets;
using Surreal.Framework;
using Surreal.Framework.Tiles;
using Surreal.Graphics;
using Surreal.Platform;

namespace Prelude {
  public sealed class PreludeGame : GameJam<PreludeGame> {
    private static readonly Matrix4x4 ProjectionView = Matrix4x4.CreateOrthographic(1920f, 1080f, 0.1f, 300f);

    public static void Main() => Start<PreludeGame>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "Prelude of the Chambered",
                IsVsyncEnabled = false,
                ShowFPSInTitle = true
            }
        },
    });

    protected override void RegisterAssetLoaders(AssetManager assets) {
      base.RegisterAssetLoaders(assets);

      assets.RegisterLoader(new TileMap<Tile>.TmxLoader(
          palette: Tile.Palette,
          converter: gid => Tile.Palette[(ushort) gid],
          defaultTile: Tile.Void
      ));
    }

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));
    }

    protected override void Draw(GameTime time) {
      GraphicsDevice.Clear(Color.Black);
      SpriteBatch.Begin(in ProjectionView);

      base.Draw(time);

      SpriteBatch.End();
    }
  }
}