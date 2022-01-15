using System.Collections.ObjectModel;
using Surreal.Utilities;

namespace Surreal.Controls;

/// <summary>An editor for graphs.</summary>
public partial class GraphEditor
{
  public GraphEditor()
  {
    InitializeComponent();
    DataContext = ViewModel;
  }

  public GraphEditorViewModel ViewModel { get; init; } = new();
}

/// <summary>A view model for the <see cref="GraphEditor"/>.</summary>
public sealed record GraphEditorViewModel : ViewModel
{
  private ObservableCollection<GraphNodeViewModel>       nodes              = new();
  private GraphNodeViewModel?                            selectedNode       = default;
  private ObservableCollection<GraphNodeViewModel>       selectedNodes      = new();
  private ObservableCollection<GraphConnectionViewModel> connections        = new();
  private ObservableCollection<GraphConnectionViewModel> pendingConnections = new();

  public ObservableCollection<GraphNodeViewModel> Nodes
  {
    get => nodes;
    set => SetProperty(ref nodes, value);
  }

  public GraphNodeViewModel? SelectedNode
  {
    get => selectedNode;
    set => SetProperty(ref selectedNode, value);
  }

  public ObservableCollection<GraphNodeViewModel> SelectedNodes
  {
    get => selectedNodes;
    set => SetProperty(ref selectedNodes, value);
  }

  public ObservableCollection<GraphConnectionViewModel> Connections
  {
    get => connections;
    set => SetProperty(ref connections, value);
  }

  public ObservableCollection<GraphConnectionViewModel> PendingConnections
  {
    get => pendingConnections;
    set => SetProperty(ref pendingConnections, value);
  }
}

/// <summary>A view model for a single node in a <see cref="GraphEditor"/>.</summary>
public sealed record GraphNodeViewModel : ViewModel
{
  private Vector2 position;

  public Vector2 Position
  {
    get => position;
    set => SetProperty(ref position, value);
  }
}

/// <summary>A view model for a single node in a <see cref="GraphEditor"/>.</summary>
public sealed record GraphConnectionViewModel : ViewModel
{
  private GraphNodeViewModel? source;
  private GraphNodeViewModel? target;

  public GraphNodeViewModel? Source
  {
    get => source;
    set => SetProperty(ref source, value);
  }

  public GraphNodeViewModel? Target
  {
    get => target;
    set => SetProperty(ref target, value);
  }
}
