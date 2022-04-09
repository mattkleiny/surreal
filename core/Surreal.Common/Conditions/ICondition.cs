using Surreal.Collections;

namespace Surreal.Conditions;

/// <summary>Triggers are conditions that cause the script to execute.</summary>
public interface ICondition
{
  /// <summary>Evaluates the condition with the given context.</summary>
  bool Evaluate(in ConditionContext context);
}

/// <summary>The context for <see cref="ICondition"/> operations.</summary>
public readonly record struct ConditionContext(
  object Owner,
  IPropertyCollection Properties
);

/// <summary>Helpers for building <see cref="ICondition"/>s.</summary>
public static class Conditions
{
  /// <summary>The signature for an <see cref="ICondition.Evaluate"/> method.</summary>
  public delegate bool ConditionDelegate(ConditionContext context);

  public static ICondition True  { get; } = Constant(true);
  public static ICondition False { get; } = Constant(false);

  /// <summary>Builds a <see cref="ICondition"/> that always returns the given value.</summary>
  public static ICondition Constant(bool value) => Anonymous(_ => value);

  /// <summary>Creates a new delegate-based <see cref="ICondition"/>.</summary>
  public static ICondition Anonymous(ConditionDelegate @delegate) => new AnonymousCondition(@delegate);

  private sealed class AnonymousCondition : ICondition
  {
    private readonly ConditionDelegate @delegate;

    public AnonymousCondition(ConditionDelegate @delegate)
    {
      this.@delegate = @delegate;
    }

    public bool Evaluate(in ConditionContext context)
    {
      return @delegate.Invoke(context);
    }
  }
}
