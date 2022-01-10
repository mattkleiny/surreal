using Surreal.Mathematics;

namespace Surreal.UI.Immediate;

public static partial class Widgets
{
  public static void Label(this in PaintingContext context, Content label, Rectangle position)
  {
    throw new NotImplementedException();
  }

  public static void Label(this in PaintingContext context, ref ContentInterpolator interpolator, Rectangle position)
  {
    Label(context, interpolator.ToContent(), position);
  }

  public static void Label(this in PaintingLayout layout, Content label)
  {
    Label(layout.Context, label, layout.Rectangle);
  }

  public static void Label(this in PaintingLayout layout, ref ContentInterpolator interpolator)
  {
    Label(layout, interpolator.ToContent());
  }
}
