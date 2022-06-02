using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Tasks;

/// <summary>A <see cref="BehaviourTask"/> that embeds some sub-<see cref="IAutomata"/>.</summary>
public sealed record NestedAutomata(IAutomata Automata) : BehaviourTask
{
  protected internal override BehaviourStatus OnUpdate(in BehaviourContext context, TimeDelta deltaTime)
  {
    var innerContext = new AutomataContext(
      Owner: context.Owner,
      LevelOfDetail: context.LevelOfDetail,
      Priority: context.Priority
    );

    var status = Automata.Tick(in innerContext, deltaTime);

    return status switch
    {
      AutomataStatus.Running => BehaviourStatus.Running,
      AutomataStatus.Success => BehaviourStatus.Success,
      AutomataStatus.Failure => BehaviourStatus.Failure,

      _ => throw new InvalidOperationException($"An unrecognized status was encountered {status}"),
    };
  }
}
