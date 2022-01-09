namespace Surreal;

/// <summary>Context for <see cref="Tactic"/> operations.</summary>
public sealed record TacticContext
{
  public object? UserData { get; init; }
}

/// <summary>A set of <see cref="Tactic"/>s that can be evaluated and executed.</summary>
public sealed record Strategy(string Name)
{
  public List<Tactic> Tactics { get; init; } = new();

  public ValueTask Evaluate(TacticContext context)
  {
    foreach (var tactic in Tactics)
    {
      if (tactic.CanExecute(context))
      {
        return tactic.Execute(context);
      }
    }

    return ValueTask.CompletedTask;
  }
}

/// <summary>A single tactic that can be evaluated for it's objectives.</summary>
public sealed record Tactic
{
  public List<TacticCondition> Conditions { get; init; } = new();
  public List<TacticAction>    Actions    { get; init; } = new();

  public bool CanExecute(TacticContext context)
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

  public async ValueTask Execute(TacticContext context)
  {
    foreach (var action in Actions)
    {
      if (action.CanExecute(context))
      {
        await action.Execute(context);
      }
    }
  }
}

/// <summary>Describes a possible condition for a <see cref="Tactic"/>.</summary>
public abstract record TacticCondition
{
  public abstract bool CanExecute(TacticContext context);
}

/// <summary>Describes a possible action for a <see cref="Tactic"/>.</summary>
public abstract record TacticAction
{
  public abstract bool      CanExecute(TacticContext context);
  public abstract ValueTask Execute(TacticContext context);
}
