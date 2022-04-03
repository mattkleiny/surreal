using Surreal.Mathematics;

namespace Surreal.Utilities;

/// <summary>A type that allows common software driven painting algorithms against it's contents</summary>
public interface IPaintable<in T>
{
  /// <summary>Paints a single point in the object.</summary>
  void DrawPoint(Point2 point, T value);
}

public static class PaintableExtensions
{
  /// <summary>Paints a line in the object.</summary>
  public static void DrawLine<T>(this IPaintable<T> paintable, Point2 from, Point2 to, T value)
  {
    throw new NotImplementedException();
  }

  /// <summary>Paints a triangle in the object.</summary>
  public static void DrawTriangle<T>(this IPaintable<T> paintable, Point2 a, Point2 b, Point2 c, T value)
  {
    throw new NotImplementedException();
  }

  /// <summary>Paints a quad in the object.</summary>
  public static void DrawQuad<T>(this IPaintable<T> paintable, Point2 center, Point2 size, T value)
  {
    throw new NotImplementedException();
  }
}
