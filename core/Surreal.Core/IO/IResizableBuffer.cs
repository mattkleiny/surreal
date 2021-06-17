namespace Surreal.IO {
  public interface IResizableBuffer<T> : IBuffer<T>
      where T : unmanaged {
    void Resize(int newLength);
  }
}