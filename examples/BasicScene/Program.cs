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
  var sprite = await game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/bunny.png");
  var music = await game.Assets.LoadAssetAsync<AudioClip>("Assets/External/audio/test.wav");

  pipeline.ClearColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

  var viewport = new CameraViewportNode();
  var camera = new CameraNode2D { Zoom = 100f };

  viewport.Add(camera);
  viewport.Add(new AudioPlayer2D
  {
    PlayOnReady = true,
    IsLooping = true,
    AudioClip = music
  });

  // create some bunnies
  for (int i = 0; i < 100; i++)
  {
    viewport.Add(new Bunny
    {
      Sprite = sprite
    });
  }

  scene.Add(viewport);
});

namespace BasicScene
{
  /// <summary>
  /// An example custom node.
  /// </summary>
  internal sealed class Bunny : SpriteNode2D
  {
    /// <summary>
    /// The speed of the bunny.
    /// </summary>
    public float Speed { get; set; } = 10f;

    /// <summary>
    /// The velocity of the bunny.
    /// </summary>
    public Vector2 Velocity { get; set; } = Vector2.Zero;

    protected override void OnUpdate(DeltaTime deltaTime)
    {
      base.OnUpdate(deltaTime);

      Velocity += new Vector2(
        (Random.Shared.NextFloat() - 0.5f) * Speed,
        (Random.Shared.NextFloat() - 0.5f) * Speed
      );

      GlobalPosition += Velocity * deltaTime;
    }
  }
}
