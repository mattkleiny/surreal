using Surreal.Commands;

namespace Surreal;

/// <summary>Static facade for editor functionality.</summary>
public static class Editor
{
  /// <summary>The top-level editor services.</summary>
  public static IServiceRegistry Services { get; internal set; } = null!;

  /// <summary>Gets the given workload <see cref="T"/>.</summary>
  public static T GetWorkload<T>() => Services.GetRequiredService<T>();

  /// <summary>Executes the given editor command.</summary>
  public static ValueTask ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
  {
    return Services
      .GetRequiredService<IEditorBus>()
      .ExecuteCommandAsync(command, cancellationToken);
  }
}
