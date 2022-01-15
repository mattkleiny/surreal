namespace Surreal.Controls;

/// <summary>A provider for nodes for use in the <see cref="GraphEditorWindow"/>.</summary>
public interface IGraphNodeProvider
{
}

/// <summary>A window for editing graphs.</summary>
public partial class GraphEditorWindow
{
  public GraphEditorWindow()
  {
    InitializeComponent();
  }

  public GraphEditorViewModel ViewModel => GraphEditor.ViewModel;
}
