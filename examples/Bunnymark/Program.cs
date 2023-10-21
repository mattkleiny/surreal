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
    var graphics = Game.Services.GetServiceOrThrow<IGraphicsBackend>();

    Game.Assets.AddLoader(new ImageLoader());
    Game.Assets.AddLoader(new TextureLoader(graphics));

    using var batch = new SpriteBatch(graphics);
    var bunnies = new List<Bunny>();

    await Game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/bunny.png");

    return _ =>
    {
      // update bunnies
      foreach (ref var bunny in bunnies.AsSpan())
      {
        bunny.Position += Vector2.UnitY;
      }

      // // draw bunnies
      // foreach (ref var bunny in bunnies.AsSpan())
      // {
      // }
    };
  })
});

public struct Bunny
{
  public Vector2 Position;
  public Color Tint;
}
