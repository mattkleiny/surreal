var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Screen Effects",
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
  using var effect = await game.Assets.LoadAberrationMaterialAsync();

  using var batch = new SpriteBatch(graphics);
  using var target = new RenderTarget(graphics, RenderTargetDescriptor.Default);

  var camera = new Camera
  {
    Position = new Vector2(0f, 0f),
    Size     = new Vector2(256, 144)
  };

  material.Properties.Set(Material.DefaultProjectionView, in camera.ProjectionView);
  effect.Properties.Set(Material.DefaultProjectionView, in camera.ProjectionView);

  var intensity = new MaterialProperty<float>("u_intensity");

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    if (keyboard.IsKeyDown(Key.W))
    {
      effect.Properties.Get(intensity) += 0.001f;
    }

    if (keyboard.IsKeyDown(Key.S))
    {
      effect.Properties.Get(intensity) -= 0.001f;
    }

    using (target.ActivateForScope())
    {
      graphics.ClearColorBuffer(Color.White);

      batch.Begin(material);
      batch.DrawText(
        font: font,
        text: "SCREEN EFFECTS",
        position: Vector2.Zero,
        scale: Vector2.One * 1.4f,
        color: Color.Black,
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
