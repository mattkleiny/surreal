using Surreal.Commands;

namespace Surreal;

/// <summary>Static facade for editor functionality.</summary>
public static class Editor
{
  /// <summary>The top-level editor <see cref="IServiceProvider"/>.</summary>
  public static IServiceProvider Services
  {
    get
    {
      var application = EditorApplication.Current;

      if (application == null)
      {
        throw new InvalidOperationException("The editor application is not currently available");
      }

      return application.Services;
    }
  }

  /// <summary>The top-level <see cref="IEditorBus"/>.</summary>
  public static IEditorBus Bus => Services.GetRequiredService<IEditorBus>();

  /// <summary>Gets the given workload <see cref="T"/>.</summary>
  public static T GetWorkload<T>() => Services.GetRequiredService<T>();

  /// <summary>Executes the given editor command.</summary>
  public static ValueTask ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
  {
    return Bus.ExecuteCommandAsync(command, cancellationToken);
  }
}
