using Surreal.Mathematics;

namespace Surreal.Graphics.UI;

public static class CanvasPrimitiveExtensions
{
  public static void DrawLine(this IImmediateModeCanvas canvas, Vector2 from, Vector2 to)
  {
    throw new NotImplementedException();
  }

  public static void DrawBox(this IImmediateModeCanvas canvas, Vector2 center, Vector2 size)
  {
    DrawBox(canvas, BoundingRect.Create(center, size));
  }

  public static void DrawBox(this IImmediateModeCanvas canvas, BoundingRect rectangle)
  {
    throw new NotImplementedException();
  }

  public static void DrawCircle(this IImmediateModeCanvas canvas, Vector2 center, float radius)
  {
    throw new NotImplementedException();
  }
}
