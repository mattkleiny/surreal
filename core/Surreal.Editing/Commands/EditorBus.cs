using System.Windows.Input;

namespace Surreal.Commands;

/// <summary>A bus that can receive and propagate commands and events.</summary>
public interface IEditorBus
{
  ValueTask ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>Handles a single <see cref="TCommand"/>'s execution.</summary>
public delegate ValueTask CommandHandler<in TCommand>(TCommand command, CancellationToken cancellationToken = default);

public static class CommandHandlers
{
  public static ICommand ToCommand<T>(T instance)
    => new EditorCommand<T>(() => instance);

  public static ICommand ToCommand<T>(Func<T> factory)
    => new EditorCommand<T>(factory);

  public static CommandHandler<TCommand> Create<TCommand>(Action action) => (_, _) =>
  {
    action.Invoke();

    return ValueTask.CompletedTask;
  };

  public static CommandHandler<TCommand> Create<TCommand>(Action<CancellationToken> action) => (_, cancellationToken) =>
  {
    action.Invoke(cancellationToken);

    return ValueTask.CompletedTask;
  };

  public static CommandHandler<TCommand> Create<TCommand>(Action<TCommand> action) => (command, _) =>
  {
    action.Invoke(command);

    return ValueTask.CompletedTask;
  };

  /// <summary>A <see cref="ICommand"/> that gets dispatched to the <see cref="Editor"/>.</summary>
  private sealed class EditorCommand<TCommand> : ICommand
  {
    private readonly Func<TCommand> commandFactory;

    public EditorCommand(Func<TCommand> commandFactory)
    {
      this.commandFactory = commandFactory;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
      return true;
    }

    public void Execute(object? parameter)
    {
      Editor.ExecuteCommandAsync(commandFactory());
    }
  }
}

/// <summary>The default <see cref="IEditorBus"/> implementation.</summary>
[RegisterService(typeof(IEditorBus))]
internal sealed class EditorBus : IEditorBus
{
  private readonly IServiceProvider serviceProvider;

  public EditorBus(IServiceProvider serviceProvider)
  {
    this.serviceProvider = serviceProvider;
  }

  public async ValueTask ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
  {
    foreach (var handler in serviceProvider.GetServices<CommandHandler<TCommand>>())
    {
      await handler.Invoke(command, cancellationToken);
    }
  }
}
