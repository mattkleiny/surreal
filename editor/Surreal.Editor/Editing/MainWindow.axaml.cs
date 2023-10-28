using Avalonia.Controls;
using Surreal.Editing.Common;

namespace Surreal.Editing;

/// <summary>
/// The main window for the editor.
/// </summary>
internal partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();

    DataContext = new MainWindowViewModel();
  }
}

/// <summary>
/// A view model for the <see cref="MainWindow"/>.
/// </summary>
internal sealed class MainWindowViewModel : EditorViewModel;
