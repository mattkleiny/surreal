using Surreal.Timing;

namespace Surreal.Automata;

/// <summary>Status for an <see cref="IAutomata"/> operation.</summary>
public enum AutomataStatus
{
  Running,
  Success,
  Failure,
}

/// <summary>The context for <see cref="IAutomata"/> operations.</summary>
public readonly record struct AutomataContext(LevelOfDetail LevelOfDetail, Priority Priority);

/// <summary>Abstracts over all kinds of automata that are capable of receiving discrete lifecycle events.</summary>
public interface IAutomata
{
  AutomataStatus Tick(in AutomataContext context, DeltaTime deltaTime);
}
