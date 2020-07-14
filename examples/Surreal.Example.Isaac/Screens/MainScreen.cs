using System.Numerics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Framework;
using Surreal.Framework.Scenes.Entities;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Framework.Scenes.Entities.Storage;
using Surreal.Framework.Scenes.Entities.Systems;
using Surreal.Framework.Screens;
using Surreal.Graphics.Cameras;
using Surreal.Input.Keyboard;

namespace Isaac.Screens {
  public sealed class MainScreen : GameScreen<IsaacGame>, ILoadableScreen {
    private readonly OrthographicCamera camera = new OrthographicCamera(256, 144);
    private readonly EntityScene        scene  = new EntityScene();

    public MainScreen(IsaacGame game)
      : base(game) {
    }

    protected override async Task LoadContentAsync(IAssetResolver assets) {
      await base.LoadContentAsync(assets);
    }

    public Task LoadInBackgroundAsync(IAssetResolver assets, ILoadNotifier notifier) {
      notifier.Increment(1f);

      return Task.CompletedTask;
    }

    public override void Initialize() {
      base.Initialize();

      scene.RegisterComponent(new DenseComponentStorage<Transform>());
      scene.RegisterComponent(new DenseComponentStorage<RigidBody>());
      scene.RegisterComponent(new DenseComponentStorage<Sprite>());

      scene.AddSystem(new PhysicsSystem {
        Gravity = Vector2.Zero,
      });

      scene.AddSystem(new CameraSystem(camera, Game.Host));
      scene.AddSystem(new SpriteSystem(SpriteBatch, camera));
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) {
        Game.Exit();
      }

      base.Input(time);

      scene.Input(time.DeltaTime);
    }

    public override void Update(GameTime time) {
      base.Update(time);

      scene.Update(time.DeltaTime);
    }

    public override void Draw(GameTime time) {
      base.Draw(time);

      scene.Draw(time.DeltaTime);
    }
  }
}