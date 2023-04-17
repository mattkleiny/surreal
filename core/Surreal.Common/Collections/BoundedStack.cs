namespace Surreal.Collections;

/// <summary>
/// A <see cref="Stack{T}" /> with a fixed-sized upper bound.
/// </summary>
public sealed class BoundedStack<T> : IEnumerable<T>
{
  private readonly Stack<T> _stack;

  public BoundedStack(int capacity = 0, int maxCapacity = 32)
  {
    Debug.Assert(capacity >= 0, "capacity >= 0");
    Debug.Assert(maxCapacity >= capacity, "maxCapacity >= capacity");

    _stack = new Stack<T>(capacity);

    Capacity = maxCapacity;
  }

  public int Count => _stack.Count;
  public int Capacity { get; }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

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
}

/// <summary>
/// A <see cref="ConcurrentStack{T}" /> with a fixed-sized upper bound.
/// </summary>
public sealed class BoundedConcurrentStack<T>
{
  private readonly ConcurrentStack<T> _stack;

  public BoundedConcurrentStack(int maxCapacity = 32)
  {
    _stack = new ConcurrentStack<T>();

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
}
