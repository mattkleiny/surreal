using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

internal sealed class MobilePlatformHost : IPlatformHost
{
  private readonly MobileConfiguration configuration;

  public MobilePlatformHost(MobileConfiguration configuration)
  {
    this.configuration = configuration;
  }

  public event Action<int, int>? Resized;

  public int  Width     { get; }
  public int  Height    { get; }
  public bool IsVisible { get; }
  public bool IsFocused { get; }
  public bool IsClosing { get; }

  public IServiceModule Services   { get; }
  public IDispatcher    Dispatcher { get; }

  public void Tick(DeltaTime deltaTime)
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
    throw new NotImplementedException();
  }
}
