using Avalonia;
using Avalonia.Controls;

namespace Surreal;

public partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();
    this.AttachDevTools();

    var pixels = new Color32[1920 * 1080];

    Array.Fill(pixels, Color32.Magenta);

    Viewport.OnFrameBufferChanged(new FrameBufferEvent(pixels));
  }
}
