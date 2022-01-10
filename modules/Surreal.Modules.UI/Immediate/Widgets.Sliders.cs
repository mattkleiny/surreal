using Surreal.Mathematics;

namespace Surreal.UI.Immediate;

public static partial class Widgets
{
  public static float VerticalSlider(this in PaintingContext context, float value, Rectangle position)
  {
    throw new NotImplementedException();
  }

  public static float VerticalSlider(this in PaintingLayout layout, float value)
  {
    return VerticalSlider(layout.Context, value, layout.Rectangle);
  }

  public static float HorizontalSlider(this in PaintingContext context, float value, Rectangle position)
  {
    throw new NotImplementedException();
  }

  public static float HorizontalSlider(this in PaintingLayout layout, float value)
  {
    return HorizontalSlider(layout.Context, value, layout.Rectangle);
  }
}
