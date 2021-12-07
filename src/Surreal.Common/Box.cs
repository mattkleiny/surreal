namespace Surreal;

/// <summary>A mutable heap-allocated box for a type of <see cref="T"/>.</summary>
public sealed class Box<T>
{
  private T value;

  public Box(T value)
  {
    this.value = value;
  }

  public ref T Value => ref value;

  public override string ToString()
  {
    return $"Box ({Value})";
  }
}