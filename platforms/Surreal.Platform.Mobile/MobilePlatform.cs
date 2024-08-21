using Surreal.Services;
using Surreal.Timing;

namespace Surreal;

public sealed record MobileConfiguration;

public class MobilePlatform : IPlatform
{
  public MobileConfiguration Configuration { get; } = new();

  public IPlatformHost BuildHost()
  {
    return new MobilePlatformHost();
  }
}

internal sealed class MobilePlatformHost : IPlatformHost
{
  public event Action<DeltaTime>? Update;
  public event Action<DeltaTime>? Render;

  public event Action<int, int>? Resized;

  public int Width => default;
  public int Height => default;
  public bool IsVisible => default;
  public bool IsFocused => default;
  public bool IsClosing => default;

  public void RegisterServices(IServiceRegistry services)
  {
  }

  public Task RunAsync()
  {
    return Task.CompletedTask;
  }

  public void Close()
  {
  }

  public void Dispose()
  {
  }
}
