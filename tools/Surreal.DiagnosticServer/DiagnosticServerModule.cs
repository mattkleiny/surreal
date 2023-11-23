using Surreal.Services;

namespace Surreal;

/// <summary>
/// A <see cref="IServiceModule"/> for the <see cref="DiagnosticServer"/>.
/// </summary>
public class DiagnosticServerModule : IServiceModule
{
  public int Port { get; set; } = 7337;

  public void RegisterServices(IServiceRegistry registry)
  {
    var diagnosticServer = new DiagnosticServer();

    registry.AddService(diagnosticServer);
    registry.AddService<IInitializable>(diagnosticServer);
  }
}
