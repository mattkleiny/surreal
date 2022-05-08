using Surreal.Input.Mouse;
using Surreal.Mathematics;

namespace Surreal.UI.Immediate.Controls;

/// <summary>Methods for rendering in an <see cref="ImmediateModeContext"/>.</summary>
public static class Buttons
{
  public static StyleKey StandardButtonStyle { get; } = new("button");
  public static StyleKey PrimaryButtonStyle  { get; } = new("button-primary");
  public static StyleKey CancelButtonStyle   { get; } = new("button-cancel");

  public static bool Button(this ImmediateModeContext context, Rectangle rectangle, string label)
  {
    return Button(context, rectangle, label, context.Skin.GetStyleOrDefault(StandardButtonStyle));
  }

  public static bool Button(this ImmediateModeContext context, Rectangle rectangle, string label, Style style)
  {
    var isPressed = false;

    if (rectangle.Contains(context.Mouse.Position))
    {
      context.DrawFillRect(rectangle, style.ActiveColor);

      if (context.Mouse.IsButtonPressed(MouseButton.Left))
      {
        isPressed = true;
      }
    }
    else
    {
      context.DrawFillRect(rectangle, style.InactiveColor);
    }

    return isPressed;
  }
}
