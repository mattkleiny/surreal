using Surreal.Input.Mouse;
using Surreal.UI.Events;

namespace Surreal.UI.Widgets;

/// <summary>A simple button <see cref="Widget"/> that can be pressed to actuate some event.</summary>
public class Button : Widget
{
  private Content label;

  public Content Label
  {
    get => label;
    set
    {
      label = value;
      MarkDirty();
    }
  }

  public event Action<Button>? Clicked;

  protected internal override void OnLayout()
  {
    throw new NotImplementedException();
  }

  protected internal override void OnPaint()
  {
    throw new NotImplementedException();
  }

  protected internal override void OnEvent<TEvent>(TEvent @event)
  {
    base.OnEvent(@event);

    switch (@event)
    {
      case MouseClickEvent { Button: MouseButton.Left }:
        Clicked?.Invoke(this);
        break;
    }
  }
}
