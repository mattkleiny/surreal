using Surreal.Graphs;

namespace Surreal.Controls.Graphs;

public partial class GraphEditorWindow
{
  public GraphEditorWindow()
  {
    InitializeComponent();
  }

  public GraphEditorWindow(IGraph graph)
    : this()
  {
    ViewModel.ResetFrom(graph);
  }

  public GraphEditorViewModel ViewModel => GraphEditor.ViewModel;
}
