using Surreal.Collections;

namespace Surreal;

/// <summary>Context for <see cref="Command"/> executions.</summary>
public readonly record struct CommandContext(object Owner, IPropertyCollection Properties);

/// <summary>An action that can be executed by anything in the world.</summary>
public abstract record Command
{
  public abstract ValueTask<CommandResult> ExecuteAsync(CommandContext context);
}

/// <summary>The result of a <see cref="Command"/>.</summary>
public readonly record struct CommandResult
{
  public static CommandResult Success { get; } = new() { IsSuccessful = true };
  public static CommandResult Failure { get; } = new() { IsSuccessful = false };

  public static CommandResult Alternative(Command command) => new()
  {
    IsSuccessful = false,
    Command = command,
  };

  public bool     IsSuccessful { get; init; }
  public Command? Command      { get; init; }
}
