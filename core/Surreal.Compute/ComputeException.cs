namespace Surreal.Compute;

/// <summary>Represents an error in the compute system.</summary>
public class ComputeException : Exception
{
  public ComputeException(string? message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
