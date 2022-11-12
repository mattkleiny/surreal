using Surreal.Timing;

namespace Surreal.Automata;

/// <summary>Status for an <see cref="IAutomata" /> operation.</summary>
public enum AutomataStatus
{
  Running,
  Success,
  Failure
}

/// <summary>Different priorities for thinking and decision making.</summary>
public enum Priority : byte
{
  Low,
  Medium,
  High
}

/// <summary>Possible levels of detail for decision making.</summary>
public enum LevelOfDetail : byte
{
  Minimum = 0,
  Low = 1,
  Medium = 2,
  High = 3
}

/// <summary>The context for <see cref="IAutomata" /> operations.</summary>
public readonly record struct AutomataContext(
  object Owner,
  LevelOfDetail LevelOfDetail,
  Priority Priority
);

/// <summary>Abstracts over all kinds of automata that are capable of receiving discrete lifecycle events.</summary>
public interface IAutomata
{
  AutomataStatus Tick(in AutomataContext context, TimeDelta deltaTime);
}


