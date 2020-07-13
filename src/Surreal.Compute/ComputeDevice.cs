using Surreal.Compute.SPI;

namespace Surreal.Compute {
  public sealed class ComputeDevice : IComputeDevice {
    public ComputeDevice(IComputeBackend backend) {
      Backend = backend;
    }

    public IComputeBackend Backend { get; }
  }
}