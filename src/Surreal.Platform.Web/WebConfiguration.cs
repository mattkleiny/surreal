using Microsoft.JSInterop;

namespace Surreal.Platform
{
  public sealed class WebConfiguration
  {
    public string     CanvasElementId   { get; set; } = "surreal-canvas";
    public IJSRuntime Runtime           { get; set; } = JSRuntime.Current;
    public bool       WaitForFirstFrame { get; set; } = true;
  }
}
