using System.Runtime.InteropServices;
using Surreal.Assets;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Input.Keyboard;

namespace HelloWorld;

public sealed class HelloWorldGame : PrototypeGame
{
  private ShaderProgram? shader;
  private Mesh<Vertex>? mesh;

  public static void Main() => Start<HelloWorldGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Hello, Surreal!",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override async Task LoadContentAsync(IAssetManager assets, CancellationToken cancellationToken = default)
  {
    await base.LoadContentAsync(assets, cancellationToken);

    shader = new ShaderProgram(GraphicsServer);

    if (GraphicsServer is IHasNativeShaderSupport nativeShaderSupport)
    {
      await nativeShaderSupport.CompileNativeShaderAsync(shader.Handle, "Assets/shaders/geometry.glsl", cancellationToken);
    }

    mesh = new Mesh<Vertex>(GraphicsServer);

    mesh.Vertices.Write(stackalloc Vertex[]
    {
      new Vertex(new(-0.25f, -0.25f), Color.Red),
      new Vertex(new(0, 0.25f), Color.Green),
      new Vertex(new(0.25f, -0.25f), Color.Blue),
    });
  }

  protected override void OnInput(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }

    base.OnInput(time);
  }

  protected override void OnDraw(GameTime time)
  {
    if (mesh != null && shader != null)
    {
      mesh.Draw(shader);
    }
  }

  public override void Dispose()
  {
    mesh?.Dispose();
    shader?.Dispose();

    base.Dispose();
  }

  [VisibleForTesting]
  [StructLayout(LayoutKind.Sequential)]
  private struct Vertex
  {
    [VertexDescriptor(
      Alias = "in_position",
      Count = 2,
      Type = VertexType.Float
    )]
    public Vector2 Position;

    [VertexDescriptor(
      Alias = "in_color",
      Count = 4,
      Type = VertexType.UnsignedByte,
      Normalized = true
    )]
    public Color Color;

    public Vertex(Vector2 position, Color color)
    {
      Position = position;
      Color    = color;
    }
  }
}
