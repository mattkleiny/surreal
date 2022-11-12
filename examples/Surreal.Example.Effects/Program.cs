var platform = new DesktopPlatform
{
  Configuration =
  {
    Title = "Screen Effects",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure

  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  using var font = await game.Assets.LoadDefaultBitmapFontAsync();
  using var material = await game.Assets.LoadDefaultSpriteMaterialAsync();
  using var effect = await game.Assets.LoadAberrationEffectAsync();

  using var batch = new SpriteBatch(graphics);
  using var target = new RenderTarget(graphics, RenderTargetDescriptor.Default);

  var camera = new Camera
  {
    Position = new Vector2(0f, 0f),
    Size = new Vector2(256, 144)
  };

  material.Locals.SetProperty(MaterialProperty.ProjectionView, in camera.ProjectionView);
  effect.Locals.SetProperty(MaterialProperty.ProjectionView, in camera.ProjectionView);

  game.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    effect.Intensity = MathF.Sin(time.TotalTime) * 0.05f;

    using (target.ActivateForScope())
    {
      graphics.ClearColorBuffer(Color.White);

      batch.Begin(material);
      batch.DrawText(
        font,
        "SCREEN EFFECTS",
        Vector2.Zero,
        Vector2.One * 1.4f,
        Color.Black,
        horizontalAlignment: HorizontalAlignment.Center,
        verticalAlignment: VerticalAlignment.Center
      );
      batch.Flush();
    }

    batch.Begin(effect);
    batch.Draw(target.ColorAttachment, Vector2.Zero, camera.Size);
    batch.Flush();
  });
});
