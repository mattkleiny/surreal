using Surreal.Scripting;

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
  // grab services
  var audio = context.Services.GetRequiredService<IAudioServer>();
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  // configure lua
  context.Assets.AddLoader(new ScriptLoader(new LuaScriptServer(), ".lua"));

  // load assets
  var font = await context.Assets.LoadDefaultBitmapFontAsync();
  var clip = await context.Assets.LoadAssetAsync<AudioClip>("Assets/audio/test.wav");
  var shader = await context.Assets.LoadAssetAsync<ShaderProgram>("Assets/shaders/helloworld.glsl");
  var palette1 = await context.Assets.LoadAssetAsync<ColorPalette>("Assets/palettes/club-seoul-16.pal");
  var palette2 = await context.Assets.LoadAssetAsync<ColorPalette>("Assets/palettes/kule-16.pal");
  var palette3 = await context.Assets.LoadAssetAsync<ColorPalette>("Assets/palettes/space-dust-9.pal");
  var script = await context.Assets.LoadAssetAsync<Script>("Assets/scripts/test.lua");

  using var source = new AudioSource(audio) { IsLooping = true };
  using var sprites = new SpriteBatch(graphics);

  // set-up a basic camera
  var camera = new Camera
  {
    Position = new(-size.X / 2f, -size.Y / 2f),
    Size     = new Vector2(size.X, size.Y),
  };

  source.Play(clip);

  var palette = palette3;

  context.ExecuteVariableStep(time =>
  {
    // handle input
    if (keyboard.IsKeyPressed(Key.Escape)) context.Exit();
    if (keyboard.IsKeyPressed(Key.F1)) palette = palette1;
    if (keyboard.IsKeyPressed(Key.F2)) palette = palette2;
    if (keyboard.IsKeyPressed(Key.F3)) palette = palette3;

    // update scripts
    script.ExecuteFunction("update", time.DeltaTime.Seconds);

    // render
    graphics.ClearColorBuffer(palette[0]);

    shader.SetUniform("u_projectionView", camera.ProjectionView);
    shader.SetUniform("u_texture", font.Texture, 0);

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
