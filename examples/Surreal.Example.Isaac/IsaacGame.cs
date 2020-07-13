﻿using System.Threading.Tasks;
using Isaac.Screens;
using Surreal;
using Surreal.Framework;
using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Sprites;
using Surreal.Platform;

namespace Isaac {
  public sealed class IsaacGame : GameJam<IsaacGame> {
    public static void Main() => Start<IsaacGame>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "The Binding of Isaac",
                IsVsyncEnabled = true,
                ShowFPSInTitle = true
            }
        }
    });

    protected override async Task<SpriteBatch> CreateSpriteBatchAsync(int spriteCountHint) {
      var shader = GraphicsDevice.Backend.CreateShaderProgram(
          // we use a custom sprite batch shader across the entire game; this shader supports palette shifting and other sprite effects.
          await Shader.LoadAsync(ShaderType.Vertex, "resx://Isaac/Resources/Shaders/SpriteBatch.vert.glsl"),
          await Shader.LoadAsync(ShaderType.Fragment, "resx://Isaac/Resources/Shaders/SpriteBatch.frag.glsl")
      );

      return SpriteBatch.Create(GraphicsDevice, shader, spriteCountHint);
    }

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this).LoadAsync());
    }

    protected override void Draw(GameTime time) {
      GraphicsDevice.Clear(Color.Black);

      base.Draw(time);
    }
  }
}