namespace Surreal.Utilities;

/// <summary>
/// Allows reporting progress of a long-running operation.
/// </summary>
public interface IProgressReporter
{
  static IProgressReporter Null { get; } = new NullProgressReporter();

  /// <summary>
  /// Reports the progress of the operation.
  /// </summary>
  IProgressScope StartOperation(string operation);

  /// <summary>
  /// A null implementation of <see cref="IProgressReporter"/>.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullProgressReporter : IProgressReporter
  {
    public IProgressScope StartOperation(string operation)
    {
      return IProgressScope.Null;
    }
  }
}

/// <summary>
/// Represents a scoped operation that reports progress.
/// </summary>
public interface IProgressScope : IDisposable
{
  static IProgressScope Null { get; } = new NullProgressScope();

  /// <summary>
  /// Reports the progress of the operation.
  /// </summary>
  void ReportProgress(float progress);

  /// <summary>
  /// A null implementation of <see cref="IProgressScope"/>.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullProgressScope : IProgressScope
  {
    public void ReportProgress(float progress)
    {
    }

    public void Dispose()
    {
    }
  }
}
