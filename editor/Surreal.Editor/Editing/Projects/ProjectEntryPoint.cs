namespace Surreal.Editing.Projects;

/// <summary>
/// An entry point for a <see cref="Game"/> for editor bootstrapping.
/// </summary>
public abstract class ProjectEntryPoint
{
  /// <summary>
  /// Creates a <see cref="ProjectEntryPoint"/> from the given 'Program' class
  /// </summary>
  public static ProjectEntryPoint FromProgram<TProgram>()
    where TProgram : class
  {
    return new AssemblyEntryPoint(typeof(TProgram).Assembly);
  }

  /// <summary>
  /// Creates a <see cref="ProjectEntryPoint"/> from the given <see cref="Assembly"/>.
  /// </summary>
  public static ProjectEntryPoint FromAssembly(Assembly assembly)
  {
    return new AssemblyEntryPoint(assembly);
  }

  /// <summary>
  /// Starts the project and runs it in a background thread.
  /// </summary>
  public abstract Task StartAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// A <see cref="ProjectEntryPoint"/> that uses an <see cref="Assembly"/> as the entry point.
  /// </summary>
  private sealed class AssemblyEntryPoint(Assembly assembly) : ProjectEntryPoint
  {
    /// <inheritdoc/>
    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
      var entryPoint = assembly.EntryPoint;
      if (entryPoint == null)
      {
        throw new InvalidOperationException($"Unable to find entry point for {assembly}");
      }

      return Task.Run(
        function: () =>
        {
          var parameters = new object?[] { Environment.GetCommandLineArgs() };

          return entryPoint.Invoke(null, parameters);
        },
        cancellationToken: cancellationToken
      );
    }
  }
}
