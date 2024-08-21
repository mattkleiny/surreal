using Surreal.Services;
using Surreal.Timing;

namespace Surreal;

public sealed record WebConfiguration
{
  public string CanvasSelector { get; set; } = ".surreal";
}

public class WebPlatform : IPlatform
{
  public WebConfiguration Configuration { get; } = new();

  public IPlatformHost BuildHost()
  {
    return new WebPlatformHost();
  }
}

internal sealed class WebPlatformHost : IPlatformHost
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

  public void Run()
  {
  }

  public void Close()
  {
  }

  public void Dispose()
  {
  }
}
