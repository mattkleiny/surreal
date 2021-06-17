namespace Surreal.Fibers {
  /// <summary>Encapsulates the status of a <see cref="FiberTask"/> or <see cref="FiberTask{T}"/>.</summary>
  public enum FiberTaskStatus {
    Pending,
    Succeeded,
    Canceled,
    Faulted
  }
}