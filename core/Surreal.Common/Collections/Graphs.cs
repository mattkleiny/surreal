using System.Runtime.CompilerServices;

namespace Surreal.Collections;

/// <summary>Abstracts over all possible <see cref="Graph{T}"/> types.</summary>
public interface IGraph
{
}

/// <summary>Abstracts over all possible <see cref="GraphNode{TSelf}"/> types.</summary>
public interface IGraphNode
{
}

/// <summary>A simple directed graph.</summary>
public abstract record Graph<TNode> : GraphNode<TNode>, IGraph
  where TNode : GraphNode<TNode>
{
  public IEnumerable<TNode> Nodes
  {
    get
    {
      static IEnumerable<TNode> GetChildrenRecursively(TNode self, int depth = 0, int maxDepth = int.MaxValue)
      {
        if (depth > maxDepth)
        {
          throw new InvalidOperationException("Maximum depth exceeded.");
        }

        foreach (var child in self.Children)
        {
          yield return child;

          foreach (var subChild in GetChildrenRecursively(child, depth + 1, maxDepth))
          {
            yield return subChild;
          }
        }
      }

      return GetChildrenRecursively(Unsafe.As<TNode>(this), maxDepth: 32);
    }
  }

  public IEnumerable<Connection> Connections
  {
    get
    {
      static IEnumerable<Connection> GetConnectionsRecursively(TNode self, int depth = 0, int maxDepth = int.MaxValue)
      {
        if (depth > maxDepth)
        {
          throw new InvalidOperationException("Maximum depth exceeded.");
        }

        foreach (var child in self.Children)
        {
          yield return new Connection(self, child);

          foreach (var subConnection in GetConnectionsRecursively(child, depth + 1, maxDepth))
          {
            yield return subConnection;
          }
        }
      }

      return GetConnectionsRecursively(Unsafe.As<TNode>(this), maxDepth: 32);
    }
  }

  /// <summary>A connection between two <see cref="TNode"/>s in the graph.</summary>
  public readonly record struct Connection(TNode From, TNode To);
}

/// <summary>Base class for any graph node of <see cref="TSelf"/>.</summary>
public abstract record GraphNode<TSelf> : IEnumerable<TSelf>, IGraphNode
  where TSelf : GraphNode<TSelf>
{
  public List<TSelf> Children { get; init; } = new();

  public void Add(TSelf node) => Children.Add(node);
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
