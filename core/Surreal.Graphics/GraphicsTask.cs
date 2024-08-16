namespace Surreal.Graphics;

/// <summary>
/// Used to indicate the status of an asynchronous task in the GPU.
/// </summary>
public struct GraphicsTask(GraphicsTaskCompletionSource source) : INotifyCompletion
{
  public static GraphicsTask CompletedTask => GraphicsTaskCompletionSource.Completed;

  public static GraphicsTask<T> FromResult<T>(T result) => GraphicsTaskCompletionSource<T>.FromResult(result);

  public static GraphicsTaskCompletionSource Create() => new();
  public static GraphicsTaskCompletionSource<T> Create<T>() => new();

  /// <summary>
  /// True if the operation is completed.
  /// </summary>
  public bool IsCompleted => source.IsCompleted;

  [UsedImplicitly]
  public void GetResult()
  {
    // no-op
  }

  public void OnCompleted(Action continuation)
  {
    source.AddContinuation(continuation);
  }
}

/// <summary>
/// A completion source for a <see cref="GraphicsTask"/>.
/// </summary>
public sealed class GraphicsTaskCompletionSource
{
  /// <summary>
  /// Creates a new task that is already completed with the given result.
  /// </summary>
  internal static GraphicsTaskCompletionSource Completed { get; } = new()
  {
    IsCompleted = true
  };

  public GraphicsTask Task => new(this);

  public bool IsCompleted { get; private set; }
  public object? Result { get; private set; }

  public void SignalCompletion()
  {
    IsCompleted = true;
  }

  public void SignalException(Exception exception)
  {
    IsCompleted = true;
    Result = exception;
  }

  public void AddContinuation(Action continuation)
  {
    // TODO: implement me
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator GraphicsTask(GraphicsTaskCompletionSource source) => source.Task;
}

/// <summary>
/// Used to indicate the status of an asynchronous task in the GPU.
/// </summary>
public readonly struct GraphicsTask<T>(GraphicsTaskCompletionSource<T> source) : INotifyCompletion
{
  /// <summary>
  /// True if the operation is completed.
  /// </summary>
  public bool IsCompleted => source.IsCompleted;

  [UsedImplicitly]
  public T GetResult()
  {
    if (!IsCompleted)
    {
      throw new InvalidOperationException("The task is not yet completed.");
    }

    if (source.Result is Exception exception)
    {
      throw exception;
    }

    return (T)source.Result!;
  }

  public void OnCompleted(Action continuation)
  {
    source.AddContinuation(continuation);
  }
}

/// <summary>
/// A completion source for a <see cref="GraphicsTask{T}"/>.
/// </summary>
public sealed class GraphicsTaskCompletionSource<T>
{
  /// <summary>
  /// Creates a new task that is already completed with the given result.
  /// </summary>
  internal static GraphicsTaskCompletionSource<T> FromResult(T result) => new()
  {
    IsCompleted = true,
    Result = result,
  };

  public GraphicsTask<T> Task => new(this);

  public bool IsCompleted { get; private set; }
  public object? Result { get; private set; }

  public void SignalCompletion(T result)
  {
    IsCompleted = true;
    Result = result;
  }

  public void SignalException(Exception exception)
  {
    IsCompleted = true;
    Result = exception;
  }

  public void AddContinuation(Action continuation)
  {
    // TODO: implement me
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator GraphicsTask<T>(GraphicsTaskCompletionSource<T> source) => source.Task;
}
