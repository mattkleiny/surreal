using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Surreal.Graphics.Shaders;
using Surreal.IO;

namespace Surreal.Internal.Graphics;

public class OpenTKGraphicsServerTests
{
  private GameWindow window = null!;

  [OneTimeSetUp]
  public void OnSetUp()
  {
    var gameWindowSettings = new GameWindowSettings
    {
      RenderFrequency = 0,
      UpdateFrequency = 0,
      IsMultiThreaded = false,
    };

    var nativeWindowSettings = new NativeWindowSettings
    {
      Title        = "Surreal Tests",
      Size         = new Vector2i(256, 144),
      WindowBorder = WindowBorder.Fixed,
      StartVisible = false,
    };

    window = new GameWindow(gameWindowSettings, nativeWindowSettings)
    {
      VSync = VSyncMode.On,
    };
  }

  [OneTimeTearDown]
  public void OnTearDown()
  {
    window.Close();
    window.Dispose();
  }

  [Test, Ignore("Not yet implemented")]
  [TestCase("Assets/shaders/test01.shade")]
  public async Task it_should_compile_a_shader(VirtualPath path)
  {
    var parser   = new StandardShaderParser();
    var pipeline = new OpenTKGraphicsServer();

    var shader = await parser.ParseShaderAsync(path);

    var id = pipeline.CreateShader();

    pipeline.CompileShader(id, shader);
  }
}
