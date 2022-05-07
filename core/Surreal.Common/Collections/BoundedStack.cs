namespace Surreal.Collections;

/// <summary>A <see cref="Stack{T}"/> with a fixed-sized upper bound.</summary>
public sealed class BoundedStack<T> : IEnumerable<T>
{
  private readonly Stack<T> stack;
  private readonly int maxCapacity;

  public BoundedStack(int capacity = 0, int maxCapacity = 32)
  {
    Debug.Assert(capacity >= 0, "capacity >= 0");
    Debug.Assert(maxCapacity >= capacity, "maxCapacity >= capacity");

    stack = new Stack<T>(capacity);

    this.maxCapacity = maxCapacity;
  }

  public int Count    => stack.Count;
  public int Capacity => maxCapacity;

  public bool TryPeek([MaybeNullWhen(false)] out T result)
  {
    return stack.TryPeek(out result);
  }

  public bool TryPush(T value)
  {
    if (stack.Count < maxCapacity)
    {
      stack.Push(value);
      return true;
    }

    return false;
  }

  public bool TryPop([MaybeNullWhen(false)] out T result)
  {
    return stack.TryPop(out result);
  }

  public void Clear()
  {
    stack.Clear();
  }

  public Stack<T>.Enumerator GetEnumerator() => stack.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}

/// <summary>A <see cref="ConcurrentStack{T}"/> with a fixed-sized upper bound.</summary>
public sealed class BoundedConcurrentStack<T>
{
  private readonly ConcurrentStack<T> stack;
  private readonly int maxCapacity;

  public BoundedConcurrentStack(int maxCapacity = 32)
  {
    stack = new ConcurrentStack<T>();

    this.maxCapacity = maxCapacity;
  }

  public int Count    => stack.Count;
  public int Capacity => maxCapacity;

  public bool TryPeek([MaybeNullWhen(false)] out T result)
  {
    return stack.TryPeek(out result);
  }

  public bool TryPush(T value)
  {
    if (stack.Count < maxCapacity)
    {
      stack.Push(value);
      return true;
    }

    return false;
  }

  public bool TryPop([MaybeNullWhen(false)] out T result)
  {
    return stack.TryPop(out result);
  }

  public void Clear()
  {
    stack.Clear();
  }
}
