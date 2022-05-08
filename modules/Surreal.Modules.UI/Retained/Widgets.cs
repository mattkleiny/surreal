using Surreal.Timing;

namespace Surreal.UI.Retained;

public abstract class Control : IDisposable
{
  public bool IsLayoutDirty   { get; private set; }
  public bool IsVisuallyDirty { get; private set; }
  public bool IsDisposed      { get; private set; }

  public void MarkDirty()
  {
    IsLayoutDirty   = true;
    IsVisuallyDirty = true;
  }

  protected internal virtual void OnMeasure()
  {
  }

  protected internal virtual void OnLayout()
  {
  }

  protected internal virtual void OnUpdate(TimeDelta deltaTime)
  {
  }

  protected internal virtual void OnDraw(TimeDelta deltaTime)
  {
  }

  public void Dispose()
  {
    if (!IsDisposed)
    {
      Dispose(true);

      IsDisposed = true;
    }
  }

  protected virtual void Dispose(bool managed)
  {
  }
}

public sealed class WidgetCanvas : Control
{
  public WidgetCanvas(Widget root)
  {
    Root = root;
  }

  public Widget Root { get; }
}

public abstract record Widget : IEnumerable<Widget>
{
  private readonly List<Widget> widgets = new();

  protected internal abstract Element Build();

  public void Add(Widget widget) => widgets.Add(widget);
  public void Remove(Widget widget) => widgets.Remove(widget);

  public List<Widget>.Enumerator GetEnumerator()
  {
    return widgets.GetEnumerator();
  }

  IEnumerator<Widget> IEnumerable<Widget>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}

public abstract record StatefulWidget : Widget
{
  public abstract Widget Create();

  public void SetState(Action commit)
  {
    throw new NotImplementedException();
  }

  protected internal sealed override Element Build()
  {
    throw new NotImplementedException();
  }
}

public record Fragment : Widget
{
  protected internal override Element Build()
  {
    throw new NotImplementedException();
  }
}

public record Box : Widget
{
  protected internal override Element Build()
  {
    throw new NotImplementedException();
  }
}

public record ClickArea(Action OnClicked) : Widget
{
  protected internal override Element Build()
  {
    throw new NotImplementedException();
  }
}

public record Button(string Label, Action OnClick) : Widget
{
  protected internal override Element Build()
  {
    throw new NotImplementedException();
  }
}

public record Label(string Text) : Widget
{
  protected internal override Element Build()
  {
    throw new NotImplementedException();
  }
}

public record CheckBox(string Label, bool IsChecked) : StatefulWidget
{
  private string label = Label;
  private bool isChecked = IsChecked;

  public string Label
  {
    get => label;
    set => SetState(() => label = value);
  }

  public bool IsChecked
  {
    get => isChecked;
    set => SetState(() => isChecked = value);
  }

  public override Widget Create()
  {
    return new Box
    {
      new Label(Label),
      new ClickArea(() => IsChecked = !IsChecked)
    };
  }
}

public record ListView(string Title) : Widget
{
  protected internal override Element Build()
  {
    throw new NotImplementedException();
  }
}

public record ListItem(string Label) : Widget
{
  protected internal override Element Build()
  {
    throw new NotImplementedException();
  }
}

public abstract record Element(Widget Widget)
{
  public abstract void Update(TimeDelta deltaTime);
  public abstract void Draw(TimeDelta deltaTime, IElementRenderer renderer);
}

public interface IElementRenderer
{
  void DrawLine();
  void DrawTriangle();
  void DrawTriangleFan();
  void DrawQuad();
  void DrawText();
  void DrawTexture();
  void DrawImage();
}

internal class ElementRenderQueue : IElementRenderer
{
  public void DrawLine()
  {
    throw new NotImplementedException();
  }

  public void DrawTriangle()
  {
    throw new NotImplementedException();
  }

  public void DrawTriangleFan()
  {
    throw new NotImplementedException();
  }

  public void DrawQuad()
  {
    throw new NotImplementedException();
  }

  public void DrawText()
  {
    throw new NotImplementedException();
  }

  public void DrawTexture()
  {
    throw new NotImplementedException();
  }

  public void DrawImage()
  {
    throw new NotImplementedException();
  }
}
