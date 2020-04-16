using System;

namespace Surreal.States
{
  public class FSM<TState>
    where TState : Enum
  {
    public FSM(TState initialState = default)
    {
      CurrentState = initialState;
    }

    public event Action<TState> Changed;

    public TState CurrentState  { get; private set; }
    public TState PreviousState { get; private set; }

    public void ChangeState(TState newState)
    {
      if (!newState.Equals(CurrentState))
      {
        PreviousState = CurrentState;
        CurrentState  = newState;

        Changed?.Invoke(newState);
      }
    }

    public static implicit operator TState(FSM<TState> fsm) => fsm.CurrentState;
  }
}
