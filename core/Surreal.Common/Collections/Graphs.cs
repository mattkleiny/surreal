namespace Surreal.Collections;

/// <summary>Abstracts over all possible <see cref="GraphNode{TSelf}"/> types.</summary>
public interface IGraphNode
{
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
