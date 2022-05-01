var size = new Vector2(256f, 144f);

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    Width          = (int) (size.X * 6),
    Height         = (int) (size.Y * 6)
  }
};

Game.Start(platform, async context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = context.Services.GetRequiredService<IInputServer>().GetRequiredDevice<IKeyboardDevice>();

  var font = await context.Assets.LoadDefaultBitmapFontAsync();
  var shader = await context.Assets.LoadDefaultShaderAsync();

  using var sprites = new SpriteBatch(graphics);

  // set-up a basic orthographic projection
  var projectionView =
    Matrix4x4.CreateTranslation(-size.X / 2f, -size.Y / 2f, 0f) *
    Matrix4x4.CreateOrthographic(size.X, size.Y, 0f, 100f);

  context.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    graphics.ClearColorBuffer(Color.White);

    shader.SetUniform("u_projectionView", in projectionView);
    shader.SetUniform("u_texture", font.Texture, 0);

    sprites.Begin(shader);
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
