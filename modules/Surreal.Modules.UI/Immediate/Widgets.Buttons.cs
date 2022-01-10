using Surreal.Mathematics;

namespace Surreal.UI.Immediate;

public static partial class Widgets
{
  public static bool Button(this in PaintingContext context, Content label, Rectangle position)
  {
    throw new NotImplementedException();
  }

  public static bool Button(this in PaintingContext context, ref ContentInterpolator interpolator, Rectangle position)
  {
    return Button(context, interpolator.ToContent(), position);
  }

  public static bool Button(this in PaintingLayout layout, Content label)
  {
    return Button(layout.Context, label, layout.Rectangle);
  }

  public static bool Button(this in PaintingLayout layout, ref ContentInterpolator interpolator)
  {
    return Button(layout, interpolator.ToContent());
  }
}
