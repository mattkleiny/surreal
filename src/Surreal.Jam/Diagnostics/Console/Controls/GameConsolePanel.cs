using Surreal.Graphics.Fonts;
using Surreal.UI.Controls;

namespace Surreal.Diagnostics.Console.Controls
{
  internal sealed class GameConsolePanel : Panel
  {
    public IGameConsole Console { get; }

    public GameConsolePanel(BitmapFont font, IGameConsole console)
    {
      Console = console;

      Add(new Panel
      {
        new Label(font, "Console"),
      });
    }
  }
}