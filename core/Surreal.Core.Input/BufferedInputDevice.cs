namespace Surreal.Input {
  public abstract class BufferedInputDevice<TState> : IInputDevice
      where TState : struct {
    public TState PreviousState { get; private set; }
    public TState CurrentState  { get; private set; }

    public virtual void Update() {
      PreviousState = CurrentState;
      CurrentState  = CaptureState();
    }

    protected abstract TState CaptureState();
  }
}