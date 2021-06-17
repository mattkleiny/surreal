namespace Surreal.Fibers {
  public enum FiberState {
    New,
    Running,
    Completed,
    Cancelled,
    Faulted,
  }
}