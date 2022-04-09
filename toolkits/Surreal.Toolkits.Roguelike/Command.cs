namespace Surreal;

/// <summary>An action that can be executed by anything in the world.</summary>
public abstract record Command
{
  public abstract CommandResult OnPerform();

  protected CommandResult Success(string? message = null)
  {
    return CommandResult.Success;
  }

  protected CommandResult Fail(string? message = null)
  {
    return CommandResult.Failure;
  }

  protected CommandResult Alterative(Command command)
  {
    return CommandResult.Alternative(command);
  }
}

/// <summary>The result of a <see cref="Command"/>.</summary>
public sealed record CommandResult
{
  public static CommandResult Success { get; } = new() { IsSuccessful = true, IsDone = true };
  public static CommandResult Failure { get; } = new() { IsSuccessful = false, IsDone = true };
  public static CommandResult NotDone { get; } = new() { IsSuccessful = false, IsDone = false };

  public static CommandResult Alternative(Command command)
  {
    return new()
    {
      IsSuccessful = false,
      IsDone = true,
      Action = command,
    };
  }

  public bool     IsSuccessful { get; init; }
  public bool     IsDone       { get; init; }
  public Command? Action       { get; init; }
}
