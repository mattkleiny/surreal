namespace Surreal;

/// <summary>An action that can be executed by anything in the world.</summary>
public abstract record Action
{
  public abstract ActionResult OnPerform();

  protected ActionResult Success(string? message = null)
  {
    return ActionResult.Success;
  }

  protected ActionResult Fail(string? message = null)
  {
    return ActionResult.Failure;
  }

  protected ActionResult Alterative(Action action)
  {
    return ActionResult.Alternative(action);
  }
}

/// <summary>The result of an <see cref="Action"/>.</summary>
public sealed record ActionResult
{
  public static ActionResult Success { get; } = new() { IsSuccessful = true, IsDone  = true };
  public static ActionResult Failure { get; } = new() { IsSuccessful = false, IsDone = true };
  public static ActionResult NotDone { get; } = new() { IsSuccessful = false, IsDone = false };

  public static ActionResult Alternative(Action action)
  {
    return new()
    {
      IsSuccessful = false,
      IsDone       = true,
      Action       = action,
    };
  }

  public bool    IsSuccessful { get; init; }
  public bool    IsDone       { get; init; }
  public Action? Action       { get; init; }
}
