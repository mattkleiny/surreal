using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Automata.StateMachines;

/// <summary>Status for an <see cref="State"/> operation.</summary>
public enum StateStatus
{
  Sleeping,
  Running,
  Success,
  Failure,
}

/// <summary>The context for <see cref="StateMachine"/> operations.</summary>
public readonly record struct StateContext(
  object Owner,
  IPropertyCollection Properties,
  StateMachine StateMachine,
  LevelOfDetail LevelOfDetail = LevelOfDetail.Medium,
  Priority Priority = Priority.Medium
)
{
  public void TransitionTo(State newState)
  {
    throw new NotImplementedException();
  }
}

/// <summary>An <see cref="IAutomata"/> that implements a finite state machine.</summary>
public sealed class StateMachine : IAutomata
{
  public StateMachine(object owner, State initialState)
    : this(owner, new PropertyBag(), initialState)
  {
  }

  public StateMachine(object owner, IPropertyCollection properties, State initialState)
  {
    Owner        = owner;
    Properties   = properties;
    CurrentState = initialState;
  }

  public object              Owner        { get; }
  public IPropertyCollection Properties   { get; }
  public State               CurrentState { get; private set; }

  public StateStatus Update(TimeDelta deltaTime)
  {
    var context = new StateContext(Owner, Properties, this);

    return CurrentState.Update(context, deltaTime);
  }

  AutomataStatus IAutomata.Tick(in AutomataContext context, TimeDelta deltaTime)
  {
    var status = CurrentState.Update(new StateContext(Owner, Properties, this, context.LevelOfDetail, context.Priority), deltaTime);

    return status switch
    {
      StateStatus.Sleeping => AutomataStatus.Running,
      StateStatus.Running  => AutomataStatus.Running,
      StateStatus.Success  => AutomataStatus.Success,
      StateStatus.Failure  => AutomataStatus.Failure,

      _ => throw new InvalidOperationException($"An unrecognized status was encountered {status}"),
    };
  }
}

/// <summary>Represents a single state in a <see cref="StateMachine"/>.</summary>
public abstract record State
{
  public StateStatus CurrentStatus { get; private set; }

  protected internal virtual void OnAwake(in StateContext context)
  {
  }

  protected internal virtual void OnEnter(in StateContext context)
  {
  }

  protected internal virtual void OnExit(in StateContext context)
  {
  }

  protected internal StateStatus Update(in StateContext context, TimeDelta deltaTime)
  {
    if (CurrentStatus == StateStatus.Sleeping)
    {
      OnAwake(in context);
    }

    if (CurrentStatus != StateStatus.Running)
    {
      OnEnter(in context);
    }

    CurrentStatus = OnUpdate(in context, deltaTime);

    if (CurrentStatus != StateStatus.Running)
    {
      OnExit(in context);
    }

    return CurrentStatus;
  }

  protected internal abstract StateStatus OnUpdate(in StateContext context, TimeDelta deltaTime);
}

/// <summary>A <see cref="State"/> that implements some sub-<see cref="IAutomata"/>.</summary>
public sealed record AutomataState(IAutomata Automata) : State
{
  protected internal override StateStatus OnUpdate(in StateContext context, TimeDelta deltaTime)
  {
    var status = Automata.Tick(new AutomataContext(context.LevelOfDetail, context.Priority), deltaTime);

    return status switch
    {
      AutomataStatus.Running => StateStatus.Running,
      AutomataStatus.Success => StateStatus.Success,
      AutomataStatus.Failure => StateStatus.Failure,

      _ => throw new InvalidOperationException($"An unrecognized status was encountered {status}"),
    };
  }
}
