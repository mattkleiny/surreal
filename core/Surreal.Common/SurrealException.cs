namespace Surreal;

/// <summary>Represents an error in Surreal.</summary>
public class SurrealException : Exception
{
  public SurrealException(string? message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
