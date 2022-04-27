namespace Surreal.Threading;

/// <summary>Options for thead operations.</summary>
public sealed record ThreadOptions
{
  public static ThreadOptions Default { get; } = new();

  public string         Name         { get; set; } = "Worker Thread";
  public bool           IsBackground { get; set; } = false;
  public ThreadPriority Priority     { get; set; } = ThreadPriority.Normal;

  /// <summary>Use a single threading apartment (for Win32 COM interop).</summary>
  /// <remarks>This is only applicable to Windows, and will be ignored on other platforms.</remarks>
  public bool UseSingleThreadApartment { get; set; } = false;
}

/// <summary>Static factory for common threading patterns.</summary>
public static class ThreadFactory
{
  /// <summary>Starts a new thread with the given cancellation token and returns a <see cref="Task"/> representing it's completion.</summary>
  public static Task Create(Func<Task> body)
  {
    return Create(ThreadOptions.Default, body);
  }

  /// <summary>Starts a new thread with the given cancellation token and returns a <see cref="Task"/> representing it's completion.</summary>
  public static Task Create(ThreadOptions options, Func<Task> body)
  {
    static async void Run(Func<Task> body, TaskCompletionSource completionSource)
    {
      try
      {
        await body();
        completionSource.SetResult();
      }
      catch (OperationCanceledException exception)
      {
        completionSource.SetCanceled(exception.CancellationToken);
      }
      catch (Exception exception)
      {
        completionSource.SetException(exception);
      }
    }

    var completionSource = new TaskCompletionSource();

    var thread = new Thread(_ => Run(body, completionSource))
    {
      Name = options.Name,
      IsBackground = options.IsBackground,
      Priority = options.Priority
    };

    if (OperatingSystem.IsWindows() && options.UseSingleThreadApartment)
    {
      thread.SetApartmentState(ApartmentState.STA);
    }

    thread.Start();

    return completionSource.Task;
  }

  /// <summary>Starts a new thread with the given cancellation token and returns a <see cref="Task{T}"/> representing it's completion.</summary>
  public static Task<T> Create<T>(Func<Task<T>> body)
  {
    return Create(ThreadOptions.Default, body);
  }

  /// <summary>Starts a new thread with the given cancellation token and returns a <see cref="Task{T}"/> representing it's completion.</summary>
  public static Task<T> Create<T>(ThreadOptions options, Func<Task<T>> body)
  {
    static async void Run(Func<Task<T>> body, TaskCompletionSource<T> completionSource)
    {
      try
      {
        completionSource.SetResult(await body());
      }
      catch (OperationCanceledException exception)
      {
        completionSource.SetCanceled(exception.CancellationToken);
      }
      catch (Exception exception)
      {
        completionSource.SetException(exception);
      }
    }

    var completionSource = new TaskCompletionSource<T>();

    var thread = new Thread(_ => Run(body, completionSource))
    {
      Name = options.Name,
      IsBackground = options.IsBackground,
      Priority = options.Priority
    };

    if (OperatingSystem.IsWindows() && options.UseSingleThreadApartment)
    {
      thread.SetApartmentState(ApartmentState.STA);
    }

    thread.Start();

    return completionSource.Task;
  }
}
