namespace Surreal.Compute;

/// <summary>Represents an error in the compute system.</summary>
public class ComputeException : SurrealException
{
  public ComputeException(string? message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
