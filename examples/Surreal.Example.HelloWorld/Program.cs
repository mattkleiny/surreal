var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    Width          = 1536,
    Height         = 864
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  using var font = await game.Assets.LoadDefaultBitmapFontAsync();
  using var material = await game.Assets.LoadDefaultSpriteMaterialAsync();

  using var sprites = new SpriteBatch(graphics);

  // set-up a basic orthographic projection
  var camera = new Camera
  {
    Position = Vector2.Zero,
    Size     = new Vector2(256, 144)
  };

  material.Properties.Add(Material.DefaultProjectionView, in camera.ProjectionView);

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    graphics.ClearColorBuffer(Color.White);

    sprites.Begin(material);
    sprites.DrawText(
      font: font,
      text: "HELLO, SURREAL!",
      position: Vector2.Zero,
      scale: Vector2.One,
      color: Color.Black,
      horizontalAlignment: HorizontalAlignment.Center,
      verticalAlignment: VerticalAlignment.Center
    );
    sprites.Flush();
  });
});
