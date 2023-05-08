namespace Surreal.Services;

/// <summary>
/// Exports a type as the implementation of some service, for use in auto-discovery.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RegisterServiceAttribute : Attribute
{
  public RegisterServiceAttribute(Type? serviceType = null)
  {
    ServiceType = serviceType;
  }

  public Type? ServiceType { get; }

  public void RegisterService(Type type, IServiceRegistry registry)
  {
    registry.RegisterService(ServiceType ?? type, type);
  }
}
