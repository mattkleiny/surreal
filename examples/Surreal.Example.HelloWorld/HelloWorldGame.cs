using HelloWorld;
using Surreal.Graphics.Images;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Input;
using Surreal.Input.Keyboard;

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
  // grab services
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  context.Assets.AddLoader(new ColorPaletteLoader());

  // load assets
  var shader = await context.Assets.LoadAsset<ShaderProgram>("Assets/shaders/helloworld.glsl");
  var palette = await context.Assets.LoadAsset<ColorPalette>("Assets/palettes/club-seoul-16.pal");

  // prepare resources
  using var texture = new Texture(graphics, TextureFormat.Rgba8888, TextureFilterMode.Point, TextureWrapMode.Clamp);
  using var image = new Image(256, 144);
  using var mesh = Mesh.BuildFullscreenQuad(graphics);

  var canvas = new PaletteImage(256, 144);
  var position = new Vector2(image.Width / 2f, image.Height / 2f);

  context.ExecuteVariableStep(time =>
  {
    const float speed = 120f;

    // handle input
    if (keyboard.IsKeyPressed(Key.Escape)) context.Exit();
    if (keyboard.IsKeyDown(Key.W)) position.Y -= 1 * speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.S)) position.Y += 1 * speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.A)) position.X -= 1 * speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.D)) position.X += 1 * speed * time.DeltaTime;

    // mutate the image contents periodically
    canvas.Fill(0);
    canvas.DrawCircle(position, 32, 1);
    canvas.DrawCircle(position, 16, 2);
    canvas.DrawCircle(position, 8, 3);
    canvas.CopyTo(image, palette);

    // upload image data to the GPU
    texture.WritePixels(image);

    // set shader parameters and draw the mesh
    shader.SetTexture("u_texture", texture.Handle, 0);

    mesh.Draw(shader);
  });
});
