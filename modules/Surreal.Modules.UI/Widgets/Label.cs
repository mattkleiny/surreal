namespace Surreal.UI.Widgets;

/// <summary>A simple label <see cref="Widget"/> that renders text.</summary>
public class Label : Widget
{
  private Content content;

  public Content Content
  {
    get => content;
    set
    {
      content = value;
      MarkDirty();
    }
  }

  protected internal override void OnLayout()
  {
    throw new NotImplementedException();
  }

  protected internal override void OnPaint()
  {
    throw new NotImplementedException();
  }
}
