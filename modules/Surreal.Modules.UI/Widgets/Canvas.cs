using Surreal.UI.Immediate;

namespace Surreal.UI.Widgets;

/// <summary>A canvas is a <see cref="Widget"/> that can paint immediate mode elements.</summary>
public abstract class Canvas : Widget
{
  private readonly IControlStateProvider provider = new ControlStateProvider();

  protected internal override void OnLayout()
  {
    var context = new PaintingContext
    {
      Stage         = PaintingStage.Layout,
      StateProvider = provider,
    };

    OnPaintCanvas(context);
  }

  protected internal override void OnPaint()
  {
    var context = new PaintingContext
    {
      Stage         = PaintingStage.Paint,
      StateProvider = provider,
    };

    OnPaintCanvas(context);
  }

  protected abstract void OnPaintCanvas(in PaintingContext context);

  /// <summary>The <see cref="IControlStateProvider"/> for canvas operations.</summary>
  private sealed class ControlStateProvider : IControlStateProvider
  {
    public uint GetControlId()
    {
      throw new NotImplementedException();
    }

    public ref TState GetControlState<TState>()
    {
      throw new NotImplementedException();
    }
  }
}
