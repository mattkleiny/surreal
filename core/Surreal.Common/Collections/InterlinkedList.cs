namespace Surreal.Collections;

/// <summary>Represents an element in a <see cref="InterlinkedList{TNode}" />.</summary>
public interface IInterlinkedElement<TSelf>
  where TSelf : class, IInterlinkedElement<TSelf>
{
  TSelf? Previous { get; set; }
  TSelf? Next { get; set; }
}

/// <summary>A linked list where each node contains pointers to/from each other, in-structure.</summary>
public sealed class InterlinkedList<TNode> : IEnumerable<TNode>
  where TNode : class, IInterlinkedElement<TNode>
{
  public TNode? Head { get; private set; }
  public bool IsEmpty => Head == null;

  IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public void Add(TNode newHead)
  {
    if (Head == null)
    {
      Head = newHead;
    }
    else
    {
      var oldHead = Head;

      oldHead.Previous = newHead;
      newHead.Next = oldHead;

      Head = newHead;
    }
  }

  public void Remove(TNode node)
  {
    if (node == Head)
    {
      Head = node.Next;

      if (Head != null)
      {
        Head.Previous = null;
      }
    }
    else
    {
      var prior = node.Previous;
      if (prior != null)
      {
        prior.Next = node.Next;
      }

      var next = node.Next;
      if (next != null)
      {
        next.Previous = node.Previous;
      }
    }
  }

  public void Clear()
  {
    Head = null;
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  /// <summary>Enumerates the <see cref="InterlinkedList{TNode}" /> from head to tail.</summary>
  public struct Enumerator : IEnumerator<TNode>
  {
    private readonly InterlinkedList<TNode> _list;

    public Enumerator(InterlinkedList<TNode> list)
    {
      _list = list;
      Current = default;
    }

    public TNode? Current { get; private set; }

    TNode IEnumerator<TNode>.Current => Current!;
    object IEnumerator.Current => Current!;

    public bool MoveNext()
    {
      if (Current == null)
      {
        Current = _list.Head;
        return Current != null;
      }

      Current = Current.Next;
      return Current != null;
    }

    public void Reset()
    {
      Current = null;
    }

    public void Dispose()
    {
      // no-op
    }
  }
}
