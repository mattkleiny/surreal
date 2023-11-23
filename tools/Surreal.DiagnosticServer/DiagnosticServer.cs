using Surreal.Services;
using Surreal.Threading;

namespace Surreal;

/// <summary>
/// A service that provides diagnostic information about the game.
/// </summary>
public sealed class DiagnosticServer : IInitializable, IDisposable
{
  private readonly WebApplication _application = WebApplication.CreateBuilder().Build();
  private readonly CancellationTokenSource _cancellationTokenSource = new();

  private Task? _thread;

  public DiagnosticServer()
  {
    _application.MapGet("/", () => "Ping");
  }

  public void Initialize()
  {
    var options = ThreadOptions.Default with
    {
      Name = "Diagnostic Server",
      IsBackground = true
    };

    _thread = ThreadFactory.Create(options, () => _application.RunAsync(_cancellationTokenSource.Token));
  }

  public void Dispose()
  {
    _cancellationTokenSource.Cancel();
    _thread?.Wait();
    _application.DisposeAsync().GetAwaiter().GetResult();
  }
}
