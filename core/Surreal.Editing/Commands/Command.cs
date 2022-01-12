namespace Surreal.Commands;

/// <summary>Context for <see cref="Command"/> operations.</summary>
public readonly record struct CommandContext(object UserData);

/// <summary>Base class for any command in the 'editor'.</summary>
public abstract record Command
{
  /// <summary>Executes the command asynchronously.</summary>
  public abstract ValueTask ExecuteAsync(CommandContext context);

  /// <summary>Rolls the the command back asynchronously.</summary>
  public abstract ValueTask RollbackAsync(CommandContext context);
}
