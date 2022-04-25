using System.Runtime.InteropServices;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
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

  mesh.Vertices.Write(stackalloc Vertex[]
  {
    new Vertex(new(-0.25f, -0.25f), Color.Red),
    new Vertex(new(0, 0.25f), Color.Green),
    new Vertex(new(0.25f, -0.25f), Color.Blue),
  });

  context.Execute(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    if (shader.IsReady)
    {
      // TODO: why aren't you visible on the screen? you are in the buffer
      mesh.Draw(shader);
    }
  });

  return ValueTask.CompletedTask;
});

[StructLayout(LayoutKind.Sequential)]
internal record struct Vertex(Vector2 Position, Color Color)
{
  [VertexDescriptor(
    Alias = "in_position",
    Count = 2,
    Type = VertexType.Float
  )]
  public Vector2 Position = Position;

  [VertexDescriptor(
    Alias = "in_color",
    Count = 4,
    Type = VertexType.Float
  )]
  public Color Color = Color;
}
