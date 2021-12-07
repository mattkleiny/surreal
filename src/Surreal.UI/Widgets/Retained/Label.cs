namespace Surreal.UI.Widgets.Retained;

/// <summary>A simple label <see cref="Widget"/> that renders text.</summary>
public class Label : Widget
{
  public Content Content { get; set; }

  protected internal override Layout OnComputeLayout(IRetainedModeContext context)
  {
    throw new NotImplementedException();
  }

  protected internal override void OnPaint(IRetainedModeContext context)
  {
    throw new NotImplementedException();
  }

  protected internal override void OnEvent<TEvent>(TEvent e)
  {
    throw new NotImplementedException();
  }
}
