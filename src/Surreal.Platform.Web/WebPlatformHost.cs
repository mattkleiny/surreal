using System;
using Microsoft.JSInterop;
using Surreal.Timing;

namespace Surreal.Platform {
  internal sealed class WebPlatformHost : IPlatformHost, IServiceProvider {
    public WebPlatformHost(IJSRuntime runtime, object canvas) {
      Runtime = runtime;
      Canvas  = canvas;
    }

    public IJSRuntime Runtime { get; }
    public dynamic    Canvas  { get; }

    public event Action<int, int> Resized;

    public int  Width     { get; }
    public int  Height    { get; }
    public bool IsVisible { get; }
    public bool IsFocused { get; }
    public bool IsClosing { get; }

    public IServiceProvider Services => this;

    object? IServiceProvider.GetService(Type serviceType) {
      return null;
    }

    public void Tick(DeltaTime deltaTime) {
    }

    public void Dispose() {
    }
  }
}