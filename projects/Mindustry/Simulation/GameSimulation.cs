using System.Numerics;
using Mindustry.Modules.Core.Components;
using Mindustry.Modules.Core.Systems;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Framework.Scenes.Entities.Storage;
using Surreal.Framework.Scenes.Entities.Systems;
using Surreal.Framework.Simulations;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Meshes;

namespace Mindustry.Simulation {
  public sealed class GameSimulation : EntitySimulation {
    public MindustryGame      Game   { get; }
    public OrthographicCamera Camera { get; }

    public GameSimulation(MindustryGame game, OrthographicCamera camera) {
      Game   = game;
      Camera = camera;
    }

    public override void Initialize() {
      base.Initialize();

      Scene.RegisterComponent(new DenseComponentStorage<Transform>());
      Scene.RegisterComponent(new DenseComponentStorage<RigidBody>());
      Scene.RegisterComponent(new DenseComponentStorage<Sprite>());
      Scene.RegisterComponent(new DenseComponentStorage<GeometrySprite>());

      Scene.AddSystem(new CameraSystem(Camera, Game.Host));
      Scene.AddSystem(new SpriteSystem(Game.SpriteBatch, Camera));
      Scene.AddSystem(new GeometrySystem(Game.GeometryBatch, Camera));

      var entity = Scene.CreateEntity();

      entity.Create(new Transform {
          Position = Vector2.Zero,
          Scale    = new Vector2(4f, 4f)
      });

      entity.Create(new GeometrySprite {
          Points = new[] {
              new Vector2(0f, 0f),
              new Vector2(0f, 8f),
              new Vector2(8f, 0f),
              new Vector2(8f, 8f),
          },
          Type = PrimitiveType.LineStrip
      });
    }
  }
}