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

  scene.Add(new CameraViewport
  {
    new Camera2D
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
      GlobalPosition = new Vector2(0f, 80f),
      SpawnFactory = () => new RigidBody2D
      {
        Children =
        {
          new Sprite2D
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
  internal sealed class Spawner : Node2D, IGizmoObject
  {
    private static readonly ILog Log = LogFactory.GetLog<Spawner>();

    public int MaxInstanceCount { get; set; } = 128;
    public int SpawnCount { get; set; } = 4;
    public TimeSpan SpawnRate { get; set; } = TimeSpan.FromSeconds(0.5f);
    public required Func<Node2D> SpawnFactory { get; set; }

    private IntervalTimer _spawnTimer;

    protected override void OnUpdate(DeltaTime deltaTime)
    {
      base.OnUpdate(deltaTime);

      if (Children.Count >= MaxInstanceCount)
      {
        return;
      }

      _spawnTimer.Tick(deltaTime);

      if (_spawnTimer.HasPassed(SpawnRate))
      {
        for (int i = 0; i < SpawnCount; i++)
        {
          var node = SpawnFactory();

          node.GlobalPosition = GlobalPosition;
          node.GlobalRotation = GlobalRotation;
          node.GlobalScale = GlobalScale;

          Add(node);
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
