namespace Surreal.Mechanics.Commands
{
  public readonly record struct CommandResult
  {
    public static CommandResult Success(string? message = default) => new()
    {
      IsSuccess = true,
      Message   = message
    };

    public static CommandResult Failure(string? message = default) => new()
    {
      IsSuccess = false,
      Message   = message
    };

    public static CommandResult Alternative(ICommand command) => new()
    {
      IsSuccess = false,
      Command   = command
    };

    public bool      IsSuccess { get; init; }
    public bool      IsFailure => !IsSuccess;
    public string?   Message   { get; init; }
    public ICommand? Command   { get; init; }
  }
}
