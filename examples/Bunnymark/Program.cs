Game.Start(new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Bunnymark",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080
    }
  },
  Host = GameHost.Create(async () =>
  {
    Game.Assets.AddLoader(new ImageLoader());
    Game.Assets.AddLoader(new TextureLoader(GraphicsContext.Default));

    using var batch = new SpriteBatch();

    var bunnies = new List<Bunny>();
    var sprite = await Game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/bunny.png");

    return time =>
    {
      foreach (ref var bunny in bunnies.AsSpan())
      {
        // TODO: update bunnies
      }

      foreach (ref var bunny in bunnies.AsSpan())
      {
        // TODO: draw bunnies
      }
    };
  })
});

public struct Bunny
{
  public Vector2 Position;
  public Color Tint;
}
