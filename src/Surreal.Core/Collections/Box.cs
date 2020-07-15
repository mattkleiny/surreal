namespace Surreal.Collections {
  public sealed class Box<T>
      where T : struct {
    private T value;

    public Box(T value) {
      this.value = value;
    }

    public ref T Value => ref value;

    public static implicit operator T(Box<T> box)   => box.Value;
    public static implicit operator Box<T>(T value) => new Box<T>(value);
  }
}