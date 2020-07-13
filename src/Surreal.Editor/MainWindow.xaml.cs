using Surreal.Editor.Internal;
using Surreal.Platform;

namespace Surreal.Editor {
  public partial class MainWindow {
    private IPlatformHost host;

    public MainWindow() {
      InitializeComponent();

      var windowHost = new EditorWindowHost();
      var platform = new DesktopPlatform {
          Configuration = {CustomWindow = windowHost}
      };

      Grid.Children.Add(windowHost);
    }
  }
}