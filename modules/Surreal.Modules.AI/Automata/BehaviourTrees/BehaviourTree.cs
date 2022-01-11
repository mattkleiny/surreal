using Surreal.Collections;
using Surreal.Messaging;
using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees;

// TODO: add helpers for creating interrupts, etc?
// TODO: support guard and semaphores
// TODO: support aborting children/etc

/// <summary>Status for execution of a <see cref="BehaviourNode"/>.</summary>
public enum BehaviourStatus
{
  Sleeping,
  Running,
  Success,
  Failure,
}

/// <summary>The context for <see cref="BehaviourNode"/> operations.</summary>
public sealed record BehaviourContext(
  object Owner,
  IPropertyCollection Properties,
  BehaviourTree BehaviourTree
);

/// <summary>An <see cref="IAutomata"/> that implements a behaviour tree.</summary>
public sealed class BehaviourTree : IAutomata, IMessageListener
{
  public BehaviourTree(object owner, BehaviourNode root)
    : this(owner, new PropertyCollection(), root)
  {
  }

  public BehaviourTree(object owner, IPropertyCollection properties, BehaviourNode root)
  {
    Context = new BehaviourContext(owner, properties, this);
    Root    = root;
  }

  public BehaviourContext    Context    { get; }
  public BehaviourNode       Root       { get; }
  public object              Owner      => Context.Owner;
  public IPropertyCollection Properties => Context.Properties;

  public BehaviourStatus Update(DeltaTime deltaTime)
  {
    return Root.Update(Context, deltaTime);
  }

  AutomataStatus IAutomata.Tick(DeltaTime deltaTime)
  {
    var status = Root.Update(Context, deltaTime);

    return status switch
    {
      BehaviourStatus.Sleeping => AutomataStatus.Running,
      BehaviourStatus.Running  => AutomataStatus.Running,
      BehaviourStatus.Success  => AutomataStatus.Success,
      BehaviourStatus.Failure  => AutomataStatus.Failure,

      _ => throw new InvalidOperationException($"An unrecognized status was encountered {status}"),
    };
  }

  void IMessageListener.OnMessageReceived(Message message)
  {
    Root.OnMessageReceived(message);
  }
}

/// <summary>Root-level representation of a <see cref="BehaviourTree"/>.</summary>
public abstract record BehaviourNode
{
  public BehaviourStatus CurrentStatus { get; private set; }

  public virtual void OnEnter(BehaviourContext context)
  {
  }

  public virtual void OnExit(BehaviourContext context)
  {
  }

  public virtual void OnMessageReceived(Message message)
  {
  }

  public BehaviourStatus Update(BehaviourContext context, DeltaTime deltaTime)
  {
    if (CurrentStatus != BehaviourStatus.Running)
    {
      OnEnter(context);
    }

    CurrentStatus = OnUpdate(context, deltaTime);

    if (CurrentStatus != BehaviourStatus.Running)
    {
      OnExit(context);
    }

    return CurrentStatus;
  }

  protected internal abstract BehaviourStatus OnUpdate(BehaviourContext context, DeltaTime deltaTime);
}

/// <summary>Represent a <see cref="BehaviourNode"/> that implements some composite.</summary>
public abstract record BehaviourComposite : BehaviourNode
{
  public ImmutableList<BehaviourNode> Children { get; init; } = ImmutableList<BehaviourNode>.Empty;
}

/// <summary>Represent a <see cref="BehaviourNode"/> that implements some decorator</summary>
public abstract record BehaviourDecorator(BehaviourNode Child) : BehaviourNode;

/// <summary>Represent a <see cref="BehaviourNode"/> that implements some task.</summary>
public abstract record BehaviourTask : BehaviourNode;
