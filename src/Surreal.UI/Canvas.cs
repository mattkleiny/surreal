using Surreal.UI.Immediate;
using Surreal.UI.Rendering;
using Surreal.UI.Retained;

namespace Surreal.UI;

/// <summary>A canvas for both both immediate mode and retained mode rendering.</summary>
public class Canvas : IImmediateModeContext, IRetainedModeContext
{
  public Canvas(IUserInterfaceRenderer renderer)
  {
    Renderer = renderer;
  }

  public IUserInterfaceRenderer Renderer { get; }
  public List<Widget>           Widgets  { get; } = new();
}
