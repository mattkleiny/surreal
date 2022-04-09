using Surreal.Collections;

namespace Surreal.Scripting;

/// <summary>Actions are the steps taken once triggers are activated.</summary>
public interface IAction
{
  /// <summary>Executes the action with the given context.</summary>
  ValueTask ExecuteAsync(ActionContext context);
}

/// <summary>The context for <see cref="IAction"/> operations.</summary>
public readonly record struct ActionContext(
  object Owner,
  IPropertyCollection Properties,
  CancellationToken CancellationToken = default
);

/// <summary>Helpers for building <see cref="IAction"/>s.</summary>
public static class Actions
{
  /// <summary>A no-op <see cref="IAction"/> implementation.</summary>
  public static IAction Null { get; } = Anonymous(_ => ValueTask.CompletedTask);

  /// <summary>Creates a new delegate-based <see cref="IAction"/>.</summary>
  public static IAction Anonymous(Action<ActionContext> @delegate) => Anonymous((context =>
  {
    @delegate.Invoke(context);

    return ValueTask.CompletedTask;
  }));

  /// <summary>Creates a new delegate-based <see cref="IAction"/>.</summary>
  public static IAction Anonymous(Action @delegate) => Anonymous((_ =>
  {
    @delegate.Invoke();

    return ValueTask.CompletedTask;
  }));

  /// <summary>Creates a new delegate-based <see cref="IAction"/>.</summary>
  public static IAction Anonymous(Func<ActionContext, ValueTask> @delegate) => new AnonymousAction(@delegate);

  private sealed class AnonymousAction : IAction
  {
    private readonly Func<ActionContext, ValueTask> @delegate;

    public AnonymousAction(Func<ActionContext, ValueTask> @delegate)
    {
      this.@delegate = @delegate;
    }

    public ValueTask ExecuteAsync(ActionContext context)
    {
      return @delegate.Invoke(context);
    }
  }
}
