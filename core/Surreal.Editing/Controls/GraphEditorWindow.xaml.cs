namespace Surreal.Controls;

/// <summary>A window for editing graphs.</summary>
public partial class GraphEditorWindow
{
  public GraphEditorWindow()
  {
    InitializeComponent();
  }

  public GraphEditorViewModel ViewModel => GraphEditor.ViewModel;
}
