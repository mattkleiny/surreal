using Surreal.Collections;
using Surreal.Messaging;
using Surreal.Timing;

namespace Surreal.Automata.StateMachines;

/// <summary>Status for an <see cref="State"/> operation.</summary>
public enum StateStatus
{
  Running,
  Success,
  Failure,
}

/// <summary>The context for <see cref="StateMachine"/> operations.</summary>
public sealed record StateContext(object Owner, IPropertyCollection Properties, StateMachine StateMachine);

/// <summary>An <see cref="IAutomata"/> that implements a finite state machine.</summary>
public sealed class StateMachine : IAutomata, IMessageListener
{
  public StateMachine(object owner, State initialState)
    : this(owner, new PropertyCollection(), initialState)
  {
  }

  public StateMachine(object owner, IPropertyCollection properties, State initialState)
  {
    Context = new StateContext(owner, properties, this);
  }

  public StateContext        Context      { get; }
  public object              Owner        => Context.Owner;
  public IPropertyCollection Properties   => Context.Properties;
  public State               CurrentState => throw new NotImplementedException();

  public StateStatus Update(DeltaTime deltaTime)
  {
    return CurrentState.Update(Context, deltaTime);
  }

  AutomataStatus IAutomata.Tick(DeltaTime deltaTime)
  {
    throw new NotImplementedException();
  }

  void IMessageListener.OnMessageReceived(Message message)
  {
    CurrentState.OnMessageReceived(message);
  }
}

/// <summary>Represents a single state in a <see cref="StateMachine"/>.</summary>
public abstract record State
{
  public StateStatus CurrentStatus { get; private set; }

  protected internal virtual void OnEnter(StateContext context)
  {
  }

  protected internal virtual void OnExit(StateContext context)
  {
  }

  protected internal virtual void OnMessageReceived(Message message)
  {
  }

  protected internal StateStatus Update(StateContext context, DeltaTime deltaTime)
  {
    if (CurrentStatus != StateStatus.Running)
    {
      OnEnter(context);
    }

    CurrentStatus = OnUpdate(context, deltaTime);

    if (CurrentStatus != StateStatus.Running)
    {
      OnExit(context);
    }

    return CurrentStatus;
  }

  protected internal abstract StateStatus OnUpdate(StateContext context, DeltaTime deltaTime);
}

/// <summary>A <see cref="State"/> that implements some sub-<see cref="IAutomata"/>.</summary>
public sealed record AutomataState(IAutomata Automata) : State
{
  protected internal override StateStatus OnUpdate(StateContext context, DeltaTime deltaTime)
  {
    var status = Automata.Tick(deltaTime);

    return status switch
    {
      AutomataStatus.Running => StateStatus.Running,
      AutomataStatus.Success => StateStatus.Success,
      AutomataStatus.Failure => StateStatus.Failure,

      _ => throw new InvalidOperationException($"An unrecognized status was encountered {status}"),
    };
  }
}
