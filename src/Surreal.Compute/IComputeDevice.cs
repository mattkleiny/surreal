using Surreal.Compute.SPI;

namespace Surreal.Compute {
  public interface IComputeDevice {
    IComputeBackend Backend { get; }
    IComputeFactory Factory { get; }
  }
}