using Surreal.Scripting;
using Surreal.Timing;

namespace Surreal.AI.BehaviourTrees.Tasks;

/// <summary>A <see cref="BehaviourTask"/> that executes the given <see cref="IAction"/>.</summary>
public sealed record ExecuteAction(IAction Action) : BehaviourTask
{
  protected internal override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    // TODO: execute action asynchronously in the behaviour tree

    throw new NotImplementedException();
  }
}
