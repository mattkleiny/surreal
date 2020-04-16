using Silence.Screens;
using Surreal;
using Surreal.Framework;
using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Input.Keyboard;
using Surreal.Platform;

namespace Silence
{
  public sealed class SilenceGame : GameJam<SilenceGame>
  {
    private const int Width  = 256;
    private const int Height = 144;
    private const int Scale  = 5;

    public static void Main() => Start<SilenceGame>(new Configuration
    {
      Platform = new DesktopPlatform
      {
        Configuration =
        {
          Title          = "The Silence Amongst the Stars",
          Width          = Width * Scale,
          Height         = Height * Scale,
          IsVsyncEnabled = true,
          ShowFPSInTitle = true
        }
      }
    });

    public OrthographicCamera Camera { get; } = new OrthographicCamera(Width, Height);

    protected override void Initialize()
    {
      base.Initialize();

      Screens.Push(new MainScreen(this).LoadAsync());
    }

    protected override void Begin()
    {
      base.Begin();

      SpriteBatch.Begin(in Camera.ProjectionView);
      GeometryBatch.Begin(in Camera.ProjectionView);
    }

    protected override void Input(GameTime time)
    {
      if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

      base.Input(time);
    }

    protected override void Draw(GameTime time)
    {
      GraphicsDevice.Clear(Color.Black);

      base.Draw(time);
    }

    protected override void End()
    {
      GeometryBatch.End();
      SpriteBatch.End();

      base.End();
    }
  }
}
