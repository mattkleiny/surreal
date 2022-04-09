using Surreal.UI.Events;

namespace Surreal.UI.Widgets;

/// <summary>Base class for any retained mode widget.</summary>
public abstract class Widget
{
  public void MarkDirtyLayout() => throw new NotImplementedException();
  public void MarkDirtyPaint() => throw new NotImplementedException();

  public void MarkDirty()
  {
    MarkDirtyLayout();
    MarkDirtyPaint();
  }

  protected internal abstract void OnLayout();
  protected internal abstract void OnPaint();

  protected internal virtual void OnEvent<TEvent>(TEvent @event) where TEvent : IEvent
  {
    // no-op by default
  }
}
