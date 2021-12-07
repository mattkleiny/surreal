using System.Drawing;

namespace Surreal.UI.Retained;

public class Button : Widget
{
  public Content    Label    { get; }
  public RectangleF Position { get; }

  public Button(Content label, RectangleF position)
  {
    Label    = label;
    Position = position;
  }

  public override Layout ComputeLayout(IRetainedModeContext context)
  {
    throw new NotImplementedException();
  }

  public override void OnEvent(Event @event)
  {
    throw new NotImplementedException();
  }

  public override void OnPaint(IRetainedModeContext context)
  {
    throw new NotImplementedException();
  }
}
