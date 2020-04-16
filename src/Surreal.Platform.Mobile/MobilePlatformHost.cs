using System;
using Surreal.Platform.SPI;
using Surreal.Timing;

namespace Surreal.Platform
{
  internal sealed class MobilePlatformHost : IPlatformHost, IServiceProvider
  {
    public MobilePlatformHost(IApplication application)
    {
      Application = application;
    }

    public IApplication Application { get; }

    public event Action<int, int> Resized;

    public int  Width     { get; }
    public int  Height    { get; }
    public bool IsVisible { get; }
    public bool IsFocused { get; }
    public bool IsClosing { get; }

    public IServiceProvider Services => this;

    object? IServiceProvider.GetService(Type serviceType)
    {
      return null;
    }

    public void Tick(DeltaTime deltaTime)
    {
    }

    public void Dispose()
    {
    }
  }
}
