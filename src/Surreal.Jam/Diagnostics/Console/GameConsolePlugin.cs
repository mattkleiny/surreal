using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Diagnostics.Console.Controls;
using Surreal.Framework;
using Surreal.Input.Keyboard;

namespace Surreal.Diagnostics.Console {
  public sealed class GameConsolePlugin : DiagnosticPlugin<GameJam> {
    public GameConsolePlugin(GameJam game)
        : base(game) {
    }

    public IGameConsole Console => Game.Console;

    public override async Task LoadContentAsync(IAssetResolver assets) {
      await base.LoadContentAsync(assets);

      var font = await assets.LoadDefaultFontAsync();

      Stage.Add(new GameConsolePanel(font, Console));
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Tilde)) IsVisible = !IsVisible;

      base.Input(time);
    }
  }
}