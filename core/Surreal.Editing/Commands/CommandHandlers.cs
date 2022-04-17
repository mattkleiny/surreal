using System.Windows.Input;

namespace Surreal.Commands;

/// <summary>Handles a single <see cref="TCommand"/>'s execution.</summary>
public delegate ValueTask CommandHandler<in TCommand>(TCommand command, CancellationToken cancellationToken = default);

/// <summary>Static factory for <see cref="CommandHandler{TCommand}"/>s.</summary>
public static class CommandHandlers
{
  public static ICommand ToCommand<T>(T instance)
    => new EditorCommand<T>(() => instance);

  public static ICommand ToCommand<T>(Func<T> factory)
    => new EditorCommand<T>(factory);

  public static void AddCommandHandler<TCommand>(this IServiceRegistry registry, CommandHandler<TCommand> handler)
    => registry.AddSingleton(handler);

  public static void AddCommandHandler<TCommand>(this IServiceRegistry registry, Action<TCommand> handler)
    => AddCommandHandler<TCommand>(registry, (command, _) =>
    {
      handler(command);
      return ValueTask.CompletedTask;
    });

  public static void AddCommandHandler<TCommand>(this IServiceRegistry registry, Action<CancellationToken> handler)
    => AddCommandHandler<TCommand>(registry, (_, cancellationToken) =>
    {
      handler(cancellationToken);
      return ValueTask.CompletedTask;
    });

  public static void AddCommandHandler<TCommand>(this IServiceRegistry registry, Action handler)
    => AddCommandHandler<TCommand>(registry, (_, _) =>
    {
      handler();
      return ValueTask.CompletedTask;
    });

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
