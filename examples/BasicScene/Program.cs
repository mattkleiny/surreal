using BasicScene;
using Surreal.Scenes.Spatial.Audio;
using Surreal.Scenes.Spatial.Physics;

var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Scene Graph Example",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080,
      IsTransparent = true
    }
  }
};

Game.StartScene<ForwardRenderPipeline>(configuration, async (Game game, SceneTree scene, ForwardRenderPipeline pipeline) =>
{
  pipeline.ClearColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

  var sprite = await game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/crab.png");
  var music = await game.Assets.LoadAssetAsync<AudioClip>("Assets/External/audio/test.wav");
  var palette = await game.Assets.LoadAssetAsync<ColorPalette>("resx://BasicScene/Assets/Embedded/palettes/kule-16.pal");

  var random = Random.Shared;

  scene.Add(new CameraViewportNode
  {
    new CameraNode2D
    {
      Zoom = 100f
    },
    new AudioPlayer2D
    {
      PlayOnReady = true,
      IsLooping = true,
      AudioClip = music,
      DistanceFalloff = 20f
    },
    new Spawner
    {
      Factory = () => new RigidBody2D
      {
        Velocity = new Vector2(
          x: random.NextFloat(-1f, 1f),
          y: random.NextFloat(0.2f, 1f)
        ) * random.NextFloat(1f, 50f),
        Torque = random.NextFloat(-1f, 1f),
        Children =
        {
          new SpriteNode2D
          {
            Sprite = sprite,
            Tint = palette.SelectRandom()
          }
        }
      }
    }
  });
});

namespace BasicScene
{
  internal sealed class Spawner : SceneNode2D, IGizmoObject
  {
    private static readonly ILog Log = LogFactory.GetLog<Spawner>();

    public int SpawnCount { get; set; } = 8 * 8;
    public TimeSpan SpawnRate { get; set; } = TimeSpan.FromSeconds(0.5f);
    public required Func<SceneNode2D> Factory { get; set; }

    private IntervalTimer _spawnTimer;

    protected override void OnUpdate(DeltaTime deltaTime)
    {
      base.OnUpdate(deltaTime);

      _spawnTimer.Tick(deltaTime);

      if (_spawnTimer.HasPassed(SpawnRate))
      {
        for (int i = 0; i < SpawnCount; i++)
        {
          Add(Factory());
        }

        _spawnTimer.Reset();

        Log.Trace($"There are {Children.Count} children");
      }
    }

    void IGizmoObject.RenderGizmos(in RenderFrame frame, GizmoBatch gizmos)
    {
      gizmos.DrawSolidCircle(GlobalPosition, 0.3f, Color.Yellow);
    }
  }
}
