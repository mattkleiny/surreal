namespace Surreal.Collections.Bounded;

/// <summary>
/// A <see cref="Stack{T}" /> with a fixed-sized upper bound.
/// </summary>
public sealed class BoundedStack<T> : IEnumerable<T>
{
  private readonly Stack<T> _stack;

  public BoundedStack(int capacity = 0, int maxCapacity = 32)
  {
    Debug.Assert(capacity >= 0);
    Debug.Assert(maxCapacity >= capacity);

    _stack = new Stack<T>(capacity);

    Capacity = maxCapacity;
  }

  public int Count => _stack.Count;
  public int Capacity { get; }

  public bool TryPeek([MaybeNullWhen(false)] out T result)
  {
    return _stack.TryPeek(out result);
  }

  public bool TryPush(T value)
  {
    if (_stack.Count < Capacity)
    {
      _stack.Push(value);
      return true;
    }

    return false;
  }

  public bool TryPop([MaybeNullWhen(false)] out T result)
  {
    return _stack.TryPop(out result);
  }

  public void Clear()
  {
    _stack.Clear();
  }

  public Stack<T>.Enumerator GetEnumerator()
  {
    return _stack.GetEnumerator();
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
