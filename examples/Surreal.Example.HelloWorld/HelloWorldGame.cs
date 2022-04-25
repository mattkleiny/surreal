using System.Runtime.InteropServices;
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
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  using var shader = context.Assets.LoadAsset<ShaderProgram>("Assets/shaders/helloworld.glsl");

  using var mesh = new Mesh<Vertex>(graphics);
  using var image = new Image(256, 144);
  using var texture = new Texture(graphics, TextureFormat.Rgba8888, TextureFilterMode.Point, TextureWrapMode.Clamp);

  var random = Random.Shared;

  mesh.Vertices.Write(stackalloc[]
  {
    new Vertex(new(0f, 0f), new(0f, 1f)),
    new Vertex(new(0f, 1f), new(0f, 0f)),
    new Vertex(new(1f, 1f), new(1f, 0f)),
    // new Vertex(new(1f, 0f), new(1f, 1f)),
  });

  // mesh.Indices.Write(stackalloc ushort[]
  // {
  //   0, 1, 2,
  //   2, 3, 0,
  // });

  context.Execute(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape)) context.Exit();

    graphics.ClearColorBuffer(Color.Black);

    if (shader.IsReady)
    {
      var pixels = image.Pixels;

      pixels.Fill(Color32.Clear);

      for (int y = 0; y < pixels.Height; y++)
      for (int x = 0; x < pixels.Width; x++)
      {
        pixels[x, y] = random.NextColor();
      }

      texture.WritePixels(image);

      shader.Value.SetTexture("u_texture", texture.Handle, 0);

      mesh.Draw(shader);
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
