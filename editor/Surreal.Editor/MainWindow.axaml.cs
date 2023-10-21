using Avalonia.Controls;

namespace Surreal.Editor;

/// <summary>
/// The main window for the editor.
/// </summary>
public class MainWindow : Window
{
  private Project? _project;

  public Project? Project
  {
    get => _project;
    set
    {
      _project = value;

      Title = value != null
        ? $"Surreal Editor ({value.RootPath})"
        : "Surreal Editor (no open project)";
    }
  }
}
