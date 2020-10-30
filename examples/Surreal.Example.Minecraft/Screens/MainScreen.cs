﻿using System.Threading.Tasks;
using Minecraft.Core;
using Minecraft.Core.Coordinates;
using Minecraft.Core.Generation;
using Minecraft.Core.Rendering;
using Surreal.Assets;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics.Cameras;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Linear;
using static Surreal.Mathematics.Maths;

namespace Minecraft.Screens {
  public sealed class MainScreen : GameScreen<Game>, ILoadableScreen {
    private readonly PerspectiveCamera camera = new(viewportWidth: 1920, viewportHeight: 1080) {
        Position  = V(-10f, 45f, -10f),
        Direction = V(0f, 15f, 0f),
        Far       = 10_000f
    };

    private WorldRenderer?               renderer;
    private FirstPersonCameraController? controller;

    private bool wireframe;

    public MainScreen(Game game)
        : base(game) {
    }

    public World World { get; set; } = null!;

    protected override async Task LoadContentAsync(IAssetResolver assets) {
      await base.LoadContentAsync(assets);

      renderer = await WorldRenderer.CreateAsync(GraphicsDevice);
    }

    public async Task LoadInBackgroundAsync(IAssetResolver assets, ILoadNotifier notifier) {
      World = await Task.Run(() => World.CreateFinite(
          regionsPerWorld: new Volume(1, 1, 1),
          palette: Block.Palette,
          chunkGenerator: ChunkGenerators.Partitioned(
              ChunkGenerators.Striated(
                  (0, 16, Block.Stone),
                  (17, 32, Block.Dirt),
                  (33, 48, Block.Grass)
              )
          ),
          biomeSelector: BiomeSelectors.Always(Biome.Temperate)
      ));

      notifier.Increment(1f);
    }

    public override void Initialize() {
      base.Initialize();

      camera.Updated += () => {
        var neighborhood = new Neighborhood(camera.Position, distance: 16);

        World.Neighborhood = neighborhood;

        if (renderer != null) {
          renderer.Neighborhood = neighborhood;
        }
      };

      controller = new FirstPersonCameraController(camera, Keyboard, Mouse) {
          Speed = 100f
      };
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();
      if (Keyboard.IsKeyPressed(Key.Space)) wireframe = !wireframe;

      base.Input(time);

      controller?.Input(time.DeltaTime);
      camera.Update();
    }

    public override void Draw(GameTime time) {
      renderer?.Render(
          device: GraphicsDevice,
          world: World,
          camera: camera,
          wireframe: wireframe
      );

      base.Draw(time);
    }

    public override void Dispose() {
      renderer?.Dispose();
      World?.Dispose();

      base.Dispose();
    }
  }
}