using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Tasks;

/// <summary>A <see cref="BehaviourTask"/> that embeds some sub-<see cref="IAutomata"/>.</summary>
public sealed record NestedAutomata(IAutomata Automata) : BehaviourTask
{
  protected internal override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    var status = Automata.Tick(new AutomataContext(context.LevelOfDetail, context.Priority), deltaTime);

    return status switch
    {
      AutomataStatus.Running => BehaviourStatus.Running,
      AutomataStatus.Success => BehaviourStatus.Success,
      AutomataStatus.Failure => BehaviourStatus.Failure,

      _ => throw new InvalidOperationException($"An unrecognized status was encountered {status}"),
    };
  }

  protected internal override void OnMessageReceived(Message message)
  {
    base.OnMessageReceived(message);

    if (Automata is IMessageListener listener)
    {
      listener.OnMessageReceived(message);
    }
  }
}
