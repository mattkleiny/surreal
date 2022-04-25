using System.Runtime.InteropServices;
using HelloWorld;
using Surreal.Graphics.Images;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
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

await Game.Start(platform, context =>
{
  // grab services
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  context.Assets.AddLoader(new ColorPaletteLoader());

  // load shader program
  using var shaderAsset = context.Assets.LoadAsset<ShaderProgram>("Assets/shaders/helloworld.glsl");
  using var palette1Asset = context.Assets.LoadAsset<ColorPalette>("Assets/palettes/club-seoul-16.pal");
  using var palette2Asset = context.Assets.LoadAsset<ColorPalette>("Assets/palettes/kule-16.pal");
  using var palette3Asset = context.Assets.LoadAsset<ColorPalette>("Assets/palettes/urbex-16.pal");

  // prepare background image
  using var image = new Image(256, 144);
  using var texture = new Texture(graphics, TextureFormat.Rgba8888, TextureFilterMode.Point, TextureWrapMode.Clamp);
  var indexed = new IndexedImage(256, 144);

  // build quad mesh
  using var mesh = new Mesh<Vertex>(graphics);

  mesh.Vertices.Write(stackalloc[]
  {
    new Vertex(new(-1f, -1f), new(0f, 1f)),
    new Vertex(new(-1f, 1f), new(0f, 0f)),
    new Vertex(new(1f, 1f), new(1f, 0f)),
    new Vertex(new(1f, -1f), new(1f, 1f)),
  });

  mesh.Indices.Write(stackalloc ushort[]
  {
    0, 1, 2,
    0, 2, 3,
  });

  var position = new Vector2(image.Width / 2f, image.Height / 2f);

  context.Execute(time =>
  {
    const float speed = 120f;

    if (keyboard.IsKeyPressed(Key.Escape)) context.Exit();

    if (keyboard.IsKeyDown(Key.W)) position.Y -= 1 * speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.S)) position.Y += 1 * speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.A)) position.X -= 1 * speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.D)) position.X += 1 * speed * time.DeltaTime;

    // TODO: remove this, sync properly on single thread (or make server multi-threadable)
    if (shaderAsset.IsReady && palette1Asset.IsReady && palette2Asset.IsReady && palette3Asset.IsReady)
    {
      var t = (float) (Math.Sin(time.TotalTime.TotalSeconds) + 1) / 2f;

      var shader = shaderAsset.Value;

      var palette1 = palette1Asset.Value;
      var palette2 = palette2Asset.Value;
      var palette3 = palette3Asset.Value;

      // mutate the image contents periodically
      indexed.Fill(0);
      indexed.DrawCircle(position, 32, 1);
      indexed.DrawCircle(position, 16, 2);
      indexed.DrawCircle(position, 8, 3);

      indexed.Blit(image, index =>
      {
        var color1 = palette1[(int) index];
        var color2 = palette2[(int) index];

        return Color32.Lerp(color1, color2, t);
      });

      texture.WritePixels(image);

      // draw the quad mesh
      shader.SetTexture("u_texture", texture.Handle, 0);
      mesh.Draw(shaderAsset);
    }
  });

  return ValueTask.CompletedTask;
});

[StructLayout(LayoutKind.Sequential)]
internal record struct Vertex(Vector2 Position, Vector2 UV)
{
  [VertexDescriptor(
    Count = 2,
    Type = VertexType.Float
  )]
  public Vector2 Position = Position;

  [VertexDescriptor(
    Count = 2,
    Type = VertexType.Float
  )]
  public Vector2 UV = UV;
}
