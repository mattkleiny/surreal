using System.Collections;
using System.Collections.Generic;

namespace Surreal.Collections
{
  public interface ILinkedNodeListElement<TSelf>
      where TSelf : class, ILinkedNodeListElement<TSelf>
  {
    TSelf? Previous { get; set; }
    TSelf? Next     { get; set; }
  }

  public sealed class LinkedNodeList<TNode> : IEnumerable<TNode>
      where TNode : class, ILinkedNodeListElement<TNode>
  {
    public TNode? Head    { get; private set; }
    public bool   IsEmpty => Head == null;

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
        newHead.Next     = oldHead;

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

    public Enumerator                     GetEnumerator() => new(this);
    IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.              GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<TNode>
    {
      private readonly LinkedNodeList<TNode> list;
      private          TNode?                current;

      public Enumerator(LinkedNodeList<TNode> list)
      {
        this.list = list;
        current   = default;
      }

      public TNode       Current => current!;
      object IEnumerator.Current => Current;

      public bool MoveNext()
      {
        if (current == null)
        {
          current = list.Head;
          return current != null;
        }

        current = current.Next;
        return current != null;
      }

      public void Reset()
      {
        current = null;
      }

      public void Dispose()
      {
      }
    }
  }
}