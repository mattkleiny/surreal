using System.Threading.Tasks;
using Mindustry.Modules.Core.Model.Schematics;
using Mindustry.Screens;
using Surreal;
using Surreal.Assets;
using Surreal.Framework;
using Surreal.Graphics;
using Surreal.Graphics.Meshes;
using Surreal.Platform;

namespace Mindustry {
  public sealed class MindustryGame : GameJam<MindustryGame> {
    public static void Main() => Start<MindustryGame>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "Mindustry",
                Width          = 1024,
                Height         = 768,
                IsVsyncEnabled = false,
                ShowFPSInTitle = true
            }
        }
    });

    public GeometryBatch GeometryBatch { get; private set; }

    protected override void RegisterAssetLoaders(AssetManager assets) {
      base.RegisterAssetLoaders(assets);

      assets.RegisterLoader(new Schematic.Loader());
    }

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));
    }

    protected override async Task LoadContentAsync(IAssetResolver assets) {
      await base.LoadContentAsync(assets);

      GeometryBatch = await GeometryBatch.CreateDefaultAsync(GraphicsDevice);
    }

    protected override void Draw(GameTime time) {
      GraphicsDevice.Clear(Color.Black);

      base.Draw(time);
    }
  }
}