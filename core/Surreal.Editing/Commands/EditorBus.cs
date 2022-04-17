namespace Surreal.Commands;

/// <summary>A bus that can receive and propagate commands and events.</summary>
public interface IEditorBus
{
  ValueTask ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>Handles a single <see cref="TCommand"/>'s execution.</summary>
public interface ICommandHandler<in TCommand>
{
  ValueTask ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
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
    foreach (var handler in serviceProvider.GetServices<ICommandHandler<TCommand>>())
    {
      await handler.ExecuteAsync(command, cancellationToken);
    }
  }
}
