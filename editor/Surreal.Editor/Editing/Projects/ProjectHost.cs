using Surreal.Hosting;
using Surreal.Threading;

namespace Surreal.Editing.Projects;

/// <summary>
/// Permits executing a game project in the context of the editor.
/// </summary>
public abstract class ProjectHost
{
  /// <summary>
  /// Creates a in-process <see cref="ProjectHost"/> from the given <see cref="Assembly"/>.
  /// <para/>
  /// It's expected that the assembly contains a static entry point method.
  /// </summary>
  public static ProjectHost InProcess(Assembly assembly)
  {
    var entryPoint = assembly.EntryPoint;
    if (entryPoint == null)
    {
      throw new InvalidOperationException($"Unable to find entry point for {assembly}");
    }

    return new InProcessHost(entryPoint);
  }

  /// <summary>
  /// Creates an out-of-process <see cref="ProjectHost"/> from the given <see cref="Assembly"/>.
  /// <para/>
  /// It's expected that the assembly will establish a connection with the editor upon loading.
  /// The given <paramref name="args"/>s will be passed like command line arguments.
  /// </summary>
  public static ProjectHost OutOfProcess(Assembly assembly, string[]? args = null)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// True if the host is running, otherwise false.
  /// </summary>
  public bool IsRunning { get; private set; }

  /// <summary>
  /// Starts the project and returns a task that represents it's execution.
  /// </summary>
  public abstract Task StartAsync(HostingContext context, CancellationToken cancellationToken = default);

  /// <summary>
  /// A <see cref="ProjectHost"/> that uses an <see cref="Assembly"/> as the entry point.
  /// </summary>
  private sealed class InProcessHost(MethodBase entryPoint) : ProjectHost
  {
    /// <inheritdoc/>
    public override Task StartAsync(HostingContext context, CancellationToken cancellationToken = default)
    {
      var options = new ThreadOptions
      {
        Name = "Game Thread",
        IsBackground = true,
        Priority = ThreadPriority.AboveNormal,
        UseSingleThreadApartment = false
      };

      return ThreadFactory.Create(options, async () =>
      {
        HostingContext.Current = context;

        cancellationToken.Register(context.NotifyCancelled);

        IsRunning = true;

        try
        {
          // run the main entry point
          var result = entryPoint.Invoke(null, new object?[]
          {
            Environment.GetCommandLineArgs()
          });

          // observe the result if it's a task
          if (result is Task task) await task;
          if (result is Task<int> taskWithCode) await taskWithCode;
        }
        finally
        {
          IsRunning = false;

          HostingContext.Current = null;
        }
      });
    }
  }
}
