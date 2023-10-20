namespace Surreal.Collections;

/// <summary>
/// Represents a node in a <see cref="IntrusiveLinkedList{TNode}" />.
/// </summary>
public interface IIntrusiveLinkedListNode<TSelf>
  where TSelf : class, IIntrusiveLinkedListNode<TSelf>
{
  TSelf? Previous { get; set; }
  TSelf? Next { get; set; }
}

/// <summary>
/// A linked list where each node contains pointers to/from each other, in-structure.
/// </summary>
public sealed class IntrusiveLinkedList<TNode> : IEnumerable<TNode>
  where TNode : class, IIntrusiveLinkedListNode<TNode>
{
  public TNode? Head { get; private set; }
  public bool IsEmpty => Head == null;

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

  IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  /// <summary>
  /// Enumerates the <see cref="IntrusiveLinkedList{TNode}" /> from head to tail.
  /// </summary>
  public struct Enumerator(IntrusiveLinkedList<TNode> list) : IEnumerator<TNode>
  {
    public TNode? Current { get; private set; } = default;

    TNode IEnumerator<TNode>.Current => Current!;
    object IEnumerator.Current => Current!;

    public bool MoveNext()
    {
      if (Current == null)
      {
        Current = list.Head;
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
