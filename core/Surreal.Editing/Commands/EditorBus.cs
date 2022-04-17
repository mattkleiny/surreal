namespace Surreal.Commands;

/// <summary>A bus that can receive and propagate commands and events.</summary>
public interface IEditorBus
{
  ValueTask ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>The default <see cref="IEditorBus"/> implementation.</summary>
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
