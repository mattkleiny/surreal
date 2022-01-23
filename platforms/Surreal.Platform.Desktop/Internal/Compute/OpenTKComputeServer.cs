using Surreal.Compute;

namespace Surreal.Internal.Compute;

internal sealed class OpenTKComputeServer : IComputeServer
{
  public ComputeHandle CreateBuffer()
  {
    throw new NotImplementedException();
  }

  public void DeleteBuffer(ComputeHandle handle)
  {
    throw new NotImplementedException();
  }

  public Memory<T> ReadBufferData<T>(ComputeHandle handle, Range range) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void WriteBufferData<T>(ComputeHandle handle, ReadOnlySpan<T> data) where T : unmanaged
  {
    throw new NotImplementedException();
  }
}
