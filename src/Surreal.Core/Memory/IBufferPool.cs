namespace Surreal.Memory
{
  public interface IBufferPool<T>
    where T : unmanaged
  {
    IDisposableBuffer<T> Rent(int count);
  }
}
