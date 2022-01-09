using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

internal sealed class MobilePlatformHost : IPlatformHost, IServiceModule
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

  public IServiceModule Services   => this;
  public IDispatcher    Dispatcher { get; } = new ImmediateDispatcher();

  public void Tick(DeltaTime deltaTime)
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
    throw new NotImplementedException();
  }

  void IServiceModule.RegisterServices(IServiceRegistry services)
  {
  }
}
