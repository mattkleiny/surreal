using System;
using Microsoft.JSInterop;

namespace Surreal.Platform
{
  public sealed class WebPlatform : IPlatform
  {
    public WebConfiguration Configuration { get; } = new WebConfiguration();

    public IPlatformHost BuildHost()
    {
      var runtime = Configuration.Runtime;
      var canvas  = ResolveCanvas(runtime, Configuration.CanvasElementId);

      return new WebPlatformHost(runtime, canvas);
    }

    private static object ResolveCanvas(IJSRuntime runtime, string elementId)
    {
      var canvas = runtime.InvokeAsync<object>($"window.getElementById('{elementId}')").Result;

      if (canvas == null)
      {
        throw new Exception($"Unable to resolve the primary display canvas with id: {elementId}");
      }

      return canvas;
    }
  }
}
