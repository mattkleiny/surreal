using Surreal.Timing;

namespace Surreal.Automata;

/// <summary>Status for an <see cref="IAutomata"/> operation.</summary>
public enum AutomataStatus
{
  Running,
  Success,
  Failure,
}

/// <summary>Abstracts over all kinds of automata that are capable of receiving discrete lifecycle events.</summary>
public interface IAutomata
{
  AutomataStatus Tick(DeltaTime deltaTime);
}
