namespace Surreal;

/// <summary>Indicates an error in the platform error of the application.</summary>
public class PlatformException : Exception
{
  public PlatformException(string message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
