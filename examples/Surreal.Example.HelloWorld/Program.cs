var size = new Vector2(256f, 144f);

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    Width          = (int) (size.X * 6),
    Height         = (int) (size.Y * 6),
  },
};

Game.Start(platform, async context =>
{
  // set-up a basic camera perspective
  var projectionView =
    Matrix4x4.CreateTranslation(-size.X / 2f, -size.Y / 2f, 0f) * // view
    Matrix4x4.CreateOrthographic(size.X, size.Y, 0f, 100f);       // projection

  // grab services
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  // load assets
  var shader = await context.Assets.LoadAsset<ShaderProgram>("Assets/shaders/helloworld.glsl");
  var palette = await context.Assets.LoadAsset<ColorPalette>("Assets/palettes/club-seoul-16.pal");
  var font = await context.Assets.LoadDefaultFontAsync();

  using var sprites = new SpriteBatch(graphics);

  context.ExecuteVariableStep(time =>
  {
    // handle input
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    // render
    graphics.ClearColorBuffer(palette[0]);

    shader.SetUniform("u_projectionView", projectionView);
    shader.SetTexture("u_texture", font.Texture, 0);

    sprites.Begin(shader);
    sprites.DrawText(
      font: font,
      text: "HELLO, SURREAL!",
      position: size / 2f,
      color: Color.Lerp(palette[1], palette[4], Maths.PingPong(time.TotalTime)),
      horizontalAlignment: HorizontalAlignment.Center,
      verticalAlignment: VerticalAlignment.Center
    );
    sprites.Flush();
  });
});
