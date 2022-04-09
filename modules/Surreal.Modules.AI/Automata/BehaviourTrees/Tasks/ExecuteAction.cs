using Surreal.Actions;
using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Tasks;

/// <summary>A <see cref="BehaviourTask"/> that executes the given <see cref="IAction"/>.</summary>
public sealed record ExecuteAction(IAction Action) : BehaviourTask
{
  protected internal override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    var actionContext = new ActionContext(context.Owner, context.Properties);

    Action.ExecuteAsync(actionContext);

    return BehaviourStatus.Running;
  }
}
