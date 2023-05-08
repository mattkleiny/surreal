namespace Surreal.Services;

/// <summary>
/// Indicates a service is not available.
/// </summary>
public class ServiceNotFoundException : Exception
{
  public ServiceNotFoundException(string message)
    : base(message)
  {
  }
}
