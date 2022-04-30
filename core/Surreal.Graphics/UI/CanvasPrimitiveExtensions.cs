using Surreal.Mathematics;

namespace Surreal.Graphics.UI;

public static class CanvasPrimitiveExtensions
{
  public static void DrawLine(this IImmediateModeContext context, Vector2 from, Vector2 to)
  {
    throw new NotImplementedException();
  }

  public static void DrawBox(this IImmediateModeContext context, Vector2 center, Vector2 size)
  {
    DrawBox(context, BoundingRect.Create(center, size));
  }

  public static void DrawBox(this IImmediateModeContext context, BoundingRect rectangle)
  {
    throw new NotImplementedException();
  }

  public static void DrawCircle(this IImmediateModeContext context, Vector2 center, float radius)
  {
    throw new NotImplementedException();
  }
}
