using Surreal.Mathematics;

namespace Surreal.UI.Immediate;

public static partial class Widgets
{
  public static bool Toggle(this in PaintingContext context, Content label, bool enabled, Rectangle position)
  {
    throw new NotImplementedException();
  }

  public static bool Toggle(this in PaintingContext context, ref ContentInterpolator interpolator, bool enabled, Rectangle position)
  {
    return Toggle(context, interpolator.ToContent(), enabled, position);
  }

  public static bool Toggle(this in PaintingLayout layout, Content label, bool enabled)
  {
    return Toggle(layout.Context, label, enabled, layout.Rectangle);
  }

  public static bool Toggle(this in PaintingLayout layout, ref ContentInterpolator interpolator, bool enabled)
  {
    return Toggle(layout, interpolator.ToContent(), enabled);
  }
}
