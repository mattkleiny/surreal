namespace Surreal.Assets;

/// <summary>Allows </summary>
public interface IProgressReporter
{
  void Increment();
}

/// <summary>An opaque token for accessing a reportable ongoing operation.</summary>
public readonly struct ProgressToken
{
  public ProgressToken(CancellationToken cancellationToken)
    : this(null, cancellationToken)
  {
  }

  public ProgressToken(IProgressReporter? progressReporter, CancellationToken cancellationToken)
  {
    ProgressReporter  = progressReporter;
    CancellationToken = cancellationToken;
  }

  public IProgressReporter? ProgressReporter  { get; }
  public CancellationToken  CancellationToken { get; }

  /// <summary>Increments the progress status of the operation.</summary>
  public void Increment()
  {
    ProgressReporter?.Increment();
  }

  public static implicit operator ProgressToken(CancellationToken cancellationToken) => new(cancellationToken);
}
