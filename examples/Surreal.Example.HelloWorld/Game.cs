using System;
using HelloWorld.Screens;
using Surreal;
using Surreal.Graphics;
using Surreal.Platform;

namespace HelloWorld {
  public sealed class Game : GameJam<Game> {
    public static void Main() => Start<Game>(new Configuration {
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

      var buffer = ComputeDevice.CreateBuffer<Color>();

      buffer.Write(stackalloc Color[100]);

      var results = buffer.Read();
    }
  }
}