var platform = new DesktopPlatform
{
  Configuration =
  {
    Title = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    Width = 256 * 6,
    Height = 144 * 6
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  using var font = await game.Assets.LoadDefaultBitmapFontAsync();
  using var material = await game.Assets.LoadDefaultSpriteMaterialAsync();
  using var batch = new SpriteBatch(graphics);

  // set-up a basic orthographic projection
  var camera = new Camera
  {
    Position = Vector2.Zero,
    Size = new Vector2(256, 144)
  };

  material.Locals.SetProperty(MaterialProperty.ProjectionView, in camera.ProjectionView);

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    graphics.ClearColorBuffer(Color.White);

    batch.Begin(material);
    batch.DrawText(
      font,
      "HELLO, WORLD!",
      Vector2.Zero,
      Vector2.One,
      Color.Black,
      horizontalAlignment: HorizontalAlignment.Center,
      verticalAlignment: VerticalAlignment.Center
    );
    batch.Flush();
  });
});
