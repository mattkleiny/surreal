var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Bunnymark",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure

  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  using var texture = await game.Assets.LoadAssetAsync<Texture>("Assets/wabbit_alpha.png");
  using var material = await game.Assets.LoadDefaultSpriteMaterialAsync();
  using var batch = new SpriteBatch(graphics, spriteCount: 8000);

  var camera = new Camera
  {
    Position = new Vector2(0f, 0f),
    Size     = new Vector2(1920f, 1080f)
  };

  var actors = new List<Bunny>();

  material.SetProperty("u_projectionView", in camera.ProjectionView);

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    if (keyboard.IsKeyDown(Key.Space))
    {
      for (int i = 0; i < 100; i++)
      {
        actors.Add(new Bunny(texture, batch));
      }
    }

    graphics.ClearColorBuffer(Color.White);

    batch.Begin(material);

    for (var i = 0; i < actors.Count; i++)
    {
      actors[i].Update();
      actors[i].Draw();
    }

    batch.Flush();
  });
});
