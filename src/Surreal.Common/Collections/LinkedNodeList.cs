using System.Collections;

namespace Surreal.Collections;

/// <summary>Represents an element in a <see cref="LinkedNodeList{TNode}"/>.</summary>
public interface ILinkedElement<TSelf>
	where TSelf : class, ILinkedElement<TSelf>
{
	TSelf? Previous { get; set; }
	TSelf? Next { get; set; }
}

/// <summary>A linked list where each node contains pointers to/from each other, in-structure.</summary>
public sealed class LinkedNodeList<TNode> : IEnumerable<TNode>
	where TNode : class, ILinkedElement<TNode>
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

	public Enumerator GetEnumerator() => new(this);
	IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator() => GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <summary>Enumerates the <see cref="LinkedNodeList{TNode}"/> from head to tail.</summary>
	public struct Enumerator : IEnumerator<TNode>
	{
		private readonly LinkedNodeList<TNode> list;
		private TNode? current;

		public Enumerator(LinkedNodeList<TNode> list)
		{
			this.list = list;
			current = default;
		}

		public TNode Current => current!;
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
