var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
  },
};

Game.Start(platform, async context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  using var shader = await context.Assets.LoadDefaultShaderAsync();
  using var texture = new Texture(graphics, TextureFormat.Rgba8888);
  using var mesh = Mesh.CreateQuad(graphics);

  var random = Random.Shared;
  var colors = new Grid<Color>(256, 144);

  context.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    graphics.ClearColorBuffer(Color.Black);

    for (int y = 0; y < colors.Height; y++)
    for (var x = 0; x < colors.Width; x++)
    {
      colors[x, y] = random.NextColor();
    }

    texture.WritePixels<Color>(256, 144, colors.Span);
    shader.SetTexture("u_texture", texture, 0);

    mesh.Draw(shader);
  });
});
