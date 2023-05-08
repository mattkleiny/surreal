using Surreal.Colors;
using Surreal.Services;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    Width = 256 * 6,
    Height = 144 * 6
  }
};

using var host = platform.BuildHost();
using var registry = new ServiceRegistry();

host.RegisterServices(registry);

var clock = new TimeDeltaClock();
var graphics = registry.GetRequiredService<IGraphicsServer>();

using var mesh = new Mesh<Vertex2>(graphics);

mesh.Vertices.Write(stackalloc Vertex2[]
{
  new Vertex2(new Vector2(0, 0), ColorF.Red, new Vector2(0, 0)),
  new Vertex2(new Vector2(1, 0), ColorF.Green, new Vector2(1, 0)),
  new Vertex2(new Vector2(1, 1), ColorF.Blue, new Vector2(1, 1)),
  new Vertex2(new Vector2(0, 1), ColorF.Yellow, new Vector2(0, 1))
});

mesh.Indices.Write(stackalloc uint[]
{
  0, 1, 2,
  2, 3, 0
});

using var shader = new ShaderProgram(graphics);

while (!host.IsClosing)
{
  var deltaTime = clock.Tick();

  graphics.ClearColorBuffer(ColorF.White);

  host.BeginFrame(deltaTime);
  host.EndFrame(deltaTime);

  graphics.FlushToDevice();
}
