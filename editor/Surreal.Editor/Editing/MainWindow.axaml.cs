using Avalonia.Controls;

namespace Surreal.Editing;

/// <summary>
/// The main window for the editor.
/// </summary>
internal partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();

    DataContext = EditorApplication.Current;
  }
}
