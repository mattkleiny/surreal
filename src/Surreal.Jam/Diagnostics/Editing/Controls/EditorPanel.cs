using Surreal.Graphics.Fonts;
using Surreal.UI.Controls;

namespace Surreal.Diagnostics.Editing.Controls {
  internal sealed class EditorPanel : Panel {
    public EditorPanel(BitmapFont font) {
      Add(new Panel {
          new Label(font, "Editor"),
      });
    }
  }
}