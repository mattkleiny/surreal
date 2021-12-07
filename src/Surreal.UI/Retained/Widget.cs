namespace Surreal.UI.Retained;

/// <summary>Base class for any retained mode widget.</summary>
public abstract class Widget
{
  public abstract Layout ComputeLayout(IRetainedModeContext context);

  public abstract void OnEvent(Event @event);
  public abstract void OnPaint(IRetainedModeContext context);
}
