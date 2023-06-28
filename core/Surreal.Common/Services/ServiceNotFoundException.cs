namespace Surreal.Services;

/// <summary>
/// Indicates a service is not available.
/// </summary>
public class ServiceNotFoundException(string message) : Exception(message);
