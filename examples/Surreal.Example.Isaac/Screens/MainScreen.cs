﻿using System.Threading.Tasks;
using Isaac.Core;
using Isaac.Core.Maps.Planning;
using Isaac.Core.Mobs;
using Surreal.Assets;
using Surreal.Framework;
using Surreal.Framework.Scenes;
using Surreal.Framework.Scenes.Actors;
using Surreal.Framework.Screens;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Mathematics;

namespace Isaac.Screens {
  public sealed class MainScreen : GameScreen<Game> {
    private const string QuickSavePath = "./quicksave.sav";

    private TextureRegion sprite = null!;

    public MainScreen(Game game)
        : base(game) {
    }

    public DungeonGenerator Generator { get; } = DungeonGenerators.Standard(Range.Of(6, 16));
    public ActorScene       Scene     { get; } = new();
    public CameraRig        CameraRig { get; } = new();

    public override void Initialize() {
      base.Initialize();

      Plugins.Add(new ScenePlugin(Scene));

      Restart();
    }

    protected override async Task LoadContentAsync(IAssetResolver assets) {
      await base.LoadContentAsync(assets);

      sprite = await assets.GetAsync<TextureRegion>("Assets/sprites/player.png");
    }

    private void Restart() {
      Scene.Actors.Clear();

      Scene.Actors.Add(CameraRig);
      Scene.Actors.Add(Generator(Game.State.Seed));
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) {
        Game.Exit();
      }

      if (Keyboard.IsKeyPressed(Key.Space)) {
        Game.State.Seed = Seed.Randomized;
        Restart();
      }

      if (Keyboard.IsKeyPressed(Key.F5)) {
        Game.SaveAsync(QuickSavePath).Wait();
      }

      if (Keyboard.IsKeyPressed(Key.F7)) {
        Game.LoadAsync(QuickSavePath).Wait();
        Restart();
      }

      base.Input(time);
    }

    public override void Update(GameTime time) {
      base.Update(time);

      ApplyHouseKeeping(Scene.Actors);
    }

    public override void Draw(GameTime time) {
      SpriteBatch.Begin(in CameraRig.Camera.ProjectionView);
      GeometryBatch.Begin(in CameraRig.Camera.ProjectionView);

      base.Draw(time);

      SpriteBatch.End();
    }

    private static void ApplyHouseKeeping(ActorList actors) {
      for (var i = 0; i < actors.Count; i++) {
        var actor = actors[i];

        if (actor is Mob mob && mob.State == Mob.States.Dead) {
          actors.Remove(mob);
        }

        ApplyHouseKeeping(actor.Children);
      }
    }
  }
}