namespace Surreal.Input;

/// <summary>An <see cref="IInputDevice"/> that is buffered frame-to-frame.</summary>
public abstract class BufferedInputDevice<TState> : IInputDevice
  where TState : notnull
{
  public TState PreviousState { get; private set; }
  public TState CurrentState  { get; private set; }

  public virtual void Update()
  {
    UpdateState();
  }

  protected void UpdateState()
  {
    PreviousState = CurrentState;
    CurrentState  = CaptureState();
  }

  protected abstract TState CaptureState();
}
