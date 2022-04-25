using Surreal.Graphics.Fonts;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;

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
  var view = Matrix4x4.CreateTranslation(-size.X / 2f, -size.Y / 2f, 0f);
  var projection = Matrix4x4.CreateOrthographic(size.X, size.Y, 0f, 100f);

  // grab services
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  // load assets
  var shader = await context.Assets.LoadAsset<ShaderProgram>("Assets/shaders/helloworld.glsl");
  var palette = await context.Assets.LoadAsset<ColorPalette>("Assets/palettes/club-seoul-16.pal");
  var font = await context.Assets.LoadDefaultFontAsync();

  // prepare resources
  using var texture = new Texture(graphics, TextureFormat.Rgba8888, TextureFilterMode.Point, TextureWrapMode.Clamp);
  using var sprites = new SpriteBatch(graphics);

  context.ExecuteVariableStep(time =>
  {
    // handle input
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    // render
    var color = Color.Lerp(palette[1], palette[4], Maths.PingPong(time.TotalTime));

    graphics.ClearColorBuffer(palette[0]);

    shader.SetUniform("u_projectionView", view * projection);
    shader.SetTexture("u_texture", font.Texture, 0);

    sprites.Begin(shader);
    sprites.DrawText(font, "HELLO, SURREAL!", size / 2f, color, TextAlignment.Center);
    sprites.Flush();
  });
});
