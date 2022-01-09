using Surreal.Graphs;

namespace Surreal.Windows;

/// <summary>A window for editing <see cref="Graph{TNode}"/>s.</summary>
public partial class GraphEditorWindow
{
  public GraphEditorWindow()
  {
    InitializeComponent();
  }
}

/// <summary>A window for editing <see cref="Graph{TNode}"/>s.</summary>
public class GraphEditorWindow<TNode> : GraphEditorWindow
{
  public GraphEditorWindow(Graph<TNode> graph)
  {
    Graph = graph;
  }

  public Graph<TNode> Graph { get; }
}
