namespace Surreal.Automata.DecisionTrees;

/// <summary>Context for <see cref="Decision"/> operations.</summary>
public sealed record DecisionContext
{
  public object? UserData { get; init; }
}

/// <summary>A set of <see cref="Decision"/>s that can be evaluated and executed.</summary>
public sealed record DecisionTree
{
  public List<Decision> Decisions { get; init; } = new();

  public ValueTask EvaluateAsync(DecisionContext context)
  {
    foreach (var tactic in Decisions)
    {
      if (tactic.CanExecute(context))
      {
        return tactic.ExecuteAsync(context);
      }
    }

    return ValueTask.CompletedTask;
  }
}

/// <summary>A single decision in a <see cref="DecisionTree"/>.</summary>
public sealed record Decision
{
  public List<Condition> Conditions { get; init; } = new();
  public List<Action>    Actions    { get; init; } = new();

  public bool CanExecute(DecisionContext context)
  {
    foreach (var condition in Conditions)
    {
      if (condition.CanExecute(context))
      {
        return true;
      }
    }

    return false;
  }

  public async ValueTask ExecuteAsync(DecisionContext context)
  {
    foreach (var action in Actions)
    {
      if (action.CanExecute(context))
      {
        await action.Execute(context);
      }
    }
  }

  /// <summary>Describes a possible condition for a <see cref="Decision"/>.</summary>
  public abstract record Condition
  {
    public abstract bool CanExecute(DecisionContext context);
  }

  /// <summary>Describes a possible action for a <see cref="Decision"/>.</summary>
  public abstract record Action
  {
    public abstract bool      CanExecute(DecisionContext context);
    public abstract ValueTask Execute(DecisionContext context);
  }
}
