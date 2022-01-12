using Surreal.Timing;

namespace Surreal.Automata.DecisionTrees;

/// <summary>An <see cref="IAutomata"/> that implements a decision tree.</summary>
public sealed record DecisionTree : IAutomata
{
  public List<DecisionNode> Decisions { get; init; } = new();

  AutomataStatus IAutomata.Tick(in AutomataContext context, DeltaTime deltaTime)
  {
    throw new NotImplementedException();
  }
}

/// <summary>A single node in a <see cref="DecisionTree"/>.</summary>
public abstract record DecisionNode;
