using BasicScene;

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
  var sprite = await game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/crab.png");
  var music = await game.Assets.LoadAssetAsync<AudioClip>("Assets/External/audio/test.wav");
  var palette = await game.Assets.LoadAssetAsync<ColorPalette>("resx://BasicScene/Assets/Embedded/palettes/kule-16.pal");

  pipeline.ClearColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
  pipeline.RequireDepthPass = false;
  pipeline.EnableGizmos = false;

  var viewport = new CameraViewportNode();

  var camera = new CameraNode2D
  {
    Zoom = 100f
  };

  var musicPlayer = new AudioPlayer2D
  {
    PlayOnReady = true,
    IsLooping = true,
    AudioClip = music,
    DistanceFalloff = 20f
  };

  viewport.Add(camera);
  viewport.Add(musicPlayer);
  viewport.Add(new Spawner
  {
    Factory = () => new Entity
    {
      Sprite = sprite,
      Tint = palette.SelectRandom(),
      Velocity = Random.Shared.NextVector2(-1f, 1f),
      RotationSpeed = Random.Shared.NextFloat(-1f, 1f),
      Bounds = new Vector2(200f, 200f)
    }
  });

  scene.Add(viewport);
});

namespace BasicScene
{
  internal sealed class Spawner : SceneNode2D, IGizmoObject
  {
    private static readonly ILog Log = LogFactory.GetLog<Spawner>();

    public int SpawnCount { get; set; } = 512;
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

  internal sealed class Entity : SpriteNode2D
  {
    public Vector2 Bounds { get; set; }
    public Vector2 Velocity { get; set; } = Vector2.Zero;
    public float RotationSpeed { get; set; } = 10f;

    protected override void OnUpdate(DeltaTime deltaTime)
    {
      base.OnUpdate(deltaTime);

      // update position and rotation
      GlobalPosition += Velocity * 100f * deltaTime;
      GlobalRotation += Angle.FromRadians(RotationSpeed * deltaTime);

      // bounce off walls
      if (GlobalPosition.X < -Bounds.X / 2f) Velocity *= new Vector2(-1f, 1f);
      if (GlobalPosition.X > Bounds.X / 2f) Velocity *= new Vector2(-1f, 1f);
      if (GlobalPosition.Y < -Bounds.Y / 2f) Velocity *= new Vector2(1f, -1f);
      if (GlobalPosition.Y > Bounds.Y / 2f) Velocity *= new Vector2(1f, -1f);

      GlobalPosition += Velocity * deltaTime;
    }
  }
}
