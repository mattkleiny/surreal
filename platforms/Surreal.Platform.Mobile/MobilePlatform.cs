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
  public event Action<int, int>? Resized;

  public int Width => default;
  public int Height => default;
  public bool IsVisible => default;
  public bool IsFocused => default;
  public bool IsClosing => default;

  public void BeginFrame(DeltaTime deltaTime)
  {
  }

  public void EndFrame(DeltaTime deltaTime)
  {
  }

  public void RegisterServices(IServiceRegistry services)
  {
  }

  public void Dispose()
  {
  }
}
