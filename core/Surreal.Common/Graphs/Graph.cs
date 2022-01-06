namespace Surreal.Graphs;

/// <summary>Abstracts over all possible <see cref="Graph{T}"/> types.</summary>
public interface IGraph
{
}

/// <summary>Abstracts over all possible <see cref="GraphNode{TSelf}"/> types.</summary>
public interface IGraphNode
{
}

/// <summary>A simple directed graph with nodes represented as an adjacency list.</summary>
public abstract class Graph<TNode> : IGraph
{
  private readonly HashSet<TNode>      nodes       = new();
  private readonly HashSet<Connection> connections = new();

  public IEnumerable<TNode> Nodes => nodes;

  public bool ContainsNode(TNode node) => nodes.Contains(node);
  public bool AddNode(TNode node)      => nodes.Add(node);
  public bool RemoveNode(TNode node)   => nodes.Remove(node);

  public bool IsConnected(TNode from, TNode to) => connections.Contains(new Connection(from, to));
  public bool Connect(TNode from, TNode to)     => connections.Add(new Connection(from, to));
  public bool Disconnect(TNode from, TNode to)  => connections.Remove(new Connection(from, to));

  /// <summary>A connection between two <see cref="TNode"/>s in the graph.</summary>
  private readonly record struct Connection(TNode From, TNode To);
}

/// <summary>Base class for any graph node of <see cref="TSelf"/>.</summary>
public abstract record GraphNode<TSelf> : IEnumerable<TSelf>, IGraphNode
  where TSelf : GraphNode<TSelf>
{
  protected List<TSelf> Children { get; } = new();

  public void Add(TSelf node)    => Children.Add(node);
  public void Remove(TSelf node) => Children.Remove(node);

  public IEnumerator<TSelf> GetEnumerator()
  {
    return Children.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
