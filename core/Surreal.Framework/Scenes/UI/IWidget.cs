using Surreal.Colors;
using Surreal.Graphics.Rendering;
using Surreal.Timing;

namespace Surreal.Scenes.UI;

/// <summary>
/// A layout for a <see cref="Widget"/>s.
/// </summary>
public sealed class WidgetLayout;

/// <summary>
/// Context for building a <see cref="Widget"/>.
/// </summary>
public readonly record struct WidgetBuildContext
{
  /// <summary>
  /// The parent <see cref="Widget"/> of the widget being built.
  /// </summary>
  public required Widget Parent { get; init; }

  /// <summary>
  /// The current box layout of the widget being built.
  /// </summary>
  public required WidgetLayout Layout { get; init; }
}

/// <summary>
/// Base class for <see cref="Widget"/>s.
/// </summary>
public abstract class Widget
{
  /// <summary>
  /// Updates this widget.
  /// </summary>
  public void Update(DeltaTime deltaTime)
  {
    // TODO: implement me
  }

  /// <summary>
  /// Renders this widget to the given <see cref="WidgetBatch"/>.
  /// </summary>
  public void Render(in RenderFrame frame, WidgetBatch batch)
  {
    // TODO: implement me
  }

  /// <summary>
  /// Builds the view for this widget.
  /// </summary>
  /// <param name="context"></param>
  protected internal abstract Widget Build(in WidgetBuildContext context);
}

public class Layout : Widget, IEnumerable<Widget>
{
  public Color BackgroundColor { get; set; } = Color.Clear;
  public float BorderThickness { get; set; }
  public Color BorderColor { get; set; } = Color.Clear;
  public float Margin { get; set; }
  public float Padding { get; set; }

  public void Add(Widget widget)
  {
    // TODO: implement me
  }

  protected internal override Widget Build(in WidgetBuildContext context)
  {
    throw new NotImplementedException();
  }

  public IEnumerator<Widget> GetEnumerator()
  {
    throw new NotImplementedException();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}

public abstract class Window : Layout;

public class FloatingWindow : Window
{
  protected internal override Widget Build(in WidgetBuildContext context)
  {
    return new StackPanel
    {
      Margin = 4f,
      Padding = 4f,
      BackgroundColor = Color.White,
      BorderColor = Color.Black,
      BorderThickness = 1f,
    };
  }
}

public class StackPanel : Layout
{
  protected internal override Widget Build(in WidgetBuildContext context)
  {
    throw new NotImplementedException();
  }
}

public class TextBlock : Widget
{
  public string Text { get; set; } = string.Empty;
  public float FontSize { get; set; } = 16f;
  public float Margin { get; set; }
  public float Padding { get; set; }

  protected internal override Widget Build(in WidgetBuildContext context)
  {
    throw new NotImplementedException();
  }
}
