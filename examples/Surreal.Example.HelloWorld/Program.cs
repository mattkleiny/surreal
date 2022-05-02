var size = new Vector2(256f, 144f);

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    Width          = (int)(size.X * 6),
    Height         = (int)(size.Y * 6)
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure

  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  var font = await game.Assets.LoadDefaultBitmapFontAsync();
  var shader = await game.Assets.LoadDefaultSpriteShaderAsync();

  using var sprites = new SpriteBatch(graphics);

  // set-up a basic orthographic projection
  var projectionView =
    Matrix4x4.CreateTranslation(-size.X / 2f, -size.Y / 2f, 0f) *
    Matrix4x4.CreateOrthographic(size.X, size.Y, 0f, 100f);

  game.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    graphics.ClearColorBuffer(Color.White);

    shader.SetUniform("u_projectionView", in projectionView);
    shader.SetUniform("u_texture", font.Texture, 0);

    sprites.Begin(shader, Matrix3x2.Identity);
    sprites.DrawText(
      font: font,
      text: "HELLO, SURREAL!",
      position: size / 2f,
      color: Color.Black,
      horizontalAlignment: HorizontalAlignment.Center,
      verticalAlignment: VerticalAlignment.Center
    );
    sprites.Flush();
  });
});
