namespace Surreal.Compute.SPI {
  public interface IComputeBackend {
    IComputeFactory Factory { get; }
  }
}