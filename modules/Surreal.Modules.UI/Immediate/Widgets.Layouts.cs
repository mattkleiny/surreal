using Surreal.Mathematics;

namespace Surreal.UI.Immediate;

public static partial class Widgets
{
  public static PaintingLayout BeginVerticalLayout(this in PaintingContext context)
  {
    throw new NotImplementedException();
  }

  public static PaintingLayout BeginHorizontalLayout(this in PaintingContext context)
  {
    throw new NotImplementedException();
  }

  public static PaintingLayout BeginLayout(this in PaintingContext context)
  {
    // TODO: implement me
    return new PaintingLayout(context)
    {
      Rectangle = new Rectangle(100, 100, 200, 200),
    };
  }
}
