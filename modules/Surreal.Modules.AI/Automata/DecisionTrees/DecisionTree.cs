using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Automata.DecisionTrees;

/// <summary>Context for <see cref="DecisionNode"/> operations.</summary>
public sealed record DecisionContext(
  object Owner,
  IPropertyCollection Properties,
  DecisionTree DecisionTree
);

/// <summary>An <see cref="IAutomata"/> that implements a decision tree.</summary>
public sealed record DecisionTree : IAutomata
{
  public List<DecisionNode> Decisions { get; init; } = new();

  AutomataStatus IAutomata.Tick(DeltaTime deltaTime)
  {
    throw new NotImplementedException();
  }
}

/// <summary>A single node in a <see cref="DecisionTree"/>.</summary>
public abstract record DecisionNode;
