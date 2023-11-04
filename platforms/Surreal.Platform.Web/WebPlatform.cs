﻿using Surreal.Timing;
using Surreal.Utilities;

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
