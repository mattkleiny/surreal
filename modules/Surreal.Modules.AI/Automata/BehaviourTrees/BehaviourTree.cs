using Surreal.Collections;
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
public readonly record struct BehaviourContext(
  object Owner,
  IPropertyCollection Properties,
  BehaviourTree BehaviourTree,
  LevelOfDetail LevelOfDetail = LevelOfDetail.Medium,
  Priority Priority = Priority.Medium
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
    Owner      = owner;
    Properties = properties;
    Root       = root;
  }

  public object              Owner      { get; }
  public IPropertyCollection Properties { get; }
  public BehaviourNode       Root       { get; }

  public BehaviourStatus Update(DeltaTime deltaTime)
  {
    var context = new BehaviourContext(Owner, Properties, this);

    return Root.Update(context, deltaTime);
  }

  AutomataStatus IAutomata.Tick(in AutomataContext context, DeltaTime deltaTime)
  {
    var status = Root.Update(new BehaviourContext(Owner, Properties, this, context.LevelOfDetail, context.Priority), deltaTime);

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

  protected internal virtual void OnAwake(in BehaviourContext context)
  {
  }

  protected internal virtual void OnEnter(in BehaviourContext context)
  {
  }

  protected internal virtual void OnExit(in BehaviourContext context)
  {
  }

  protected internal virtual void OnMessageReceived(Message message)
  {
  }

  public BehaviourStatus Update(in BehaviourContext context, DeltaTime deltaTime)
  {
    if (CurrentStatus == BehaviourStatus.Sleeping)
    {
      OnAwake(in context);
    }

    if (CurrentStatus != BehaviourStatus.Running)
    {
      OnEnter(in context);
    }

    CurrentStatus = OnUpdate(in context, deltaTime);

    if (CurrentStatus != BehaviourStatus.Running)
    {
      OnExit(in context);
    }

    return CurrentStatus;
  }

  protected internal abstract BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime);
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
