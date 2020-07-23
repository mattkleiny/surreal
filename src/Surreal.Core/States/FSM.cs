using System;
using Surreal.Utilities;

namespace Surreal.States {
  public class FSM<TState>
      where TState : unmanaged, Enum {
    public FSM(TState initialState = default) {
      CurrentState = initialState;
    }

    public event Action<TState> Changed;

    public TState CurrentState  { get; private set; }
    public TState PreviousState { get; private set; }

    public void ChangeState(TState newState) {
      if (!newState.EqualsFast(CurrentState)) {
        PreviousState = CurrentState;
        CurrentState  = newState;

        Changed?.Invoke(newState);
      }
    }

    public override string ToString() => $"{CurrentState}";

    public static implicit operator TState(FSM<TState> fsm) => fsm.CurrentState;
  }
}