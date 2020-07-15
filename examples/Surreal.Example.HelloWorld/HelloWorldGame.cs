using System;
using HelloWorld.Screens;
using Surreal;
using Surreal.Framework;
using Surreal.Graphics;
using Surreal.Platform;

namespace HelloWorld {
  public sealed class HelloWorldGame : GameJam<HelloWorldGame> {
    public static void Main() => Start<HelloWorldGame>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "Hello, Surreal!",
                IsVsyncEnabled = true,
                ShowFPSInTitle = true
            }
        }
    });

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));

      using var buffer = ComputeDevice.CreateBuffer();

      buffer.Write(stackalloc Color[100]);

      var colors = buffer.Read<Color>(Range.All);
    }

    protected override void Draw(GameTime time) {
      GraphicsDevice.Clear(Color.Black);

      base.Draw(time);
    }
  }
}