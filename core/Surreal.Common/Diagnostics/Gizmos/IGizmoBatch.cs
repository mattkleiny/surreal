using Surreal.Colors;
using Surreal.Maths;
using Surreal.Memory;

namespace Surreal.Diagnostics.Gizmos;

/// <summary>
/// Represents a type capable of rendering gizmos to the display.
/// </summary>
public interface IGizmoBatch
{
  void DrawPoint(Vector2 position, Color color);
  void DrawLine(Vector2 from, Vector2 to, Color color);
  void DrawLineStrip(SpanList<Vector2> points, Color color);

  void DrawSolidTriangle(Vector2 a, Vector2 b, Vector2 c, Color color);
  void DrawWireTriangle(Vector2 a, Vector2 b, Vector2 c, Color color);

  void DrawSolidQuad(Rectangle rectangle, Color color);
  void DrawSolidQuad(Vector2 center, Vector2 size, Color color);
  void DrawWireQuad(Rectangle rectangle, Color color);
  void DrawWireQuad(Vector2 center, Vector2 size, Color color);

  void DrawSolidCircle(Vector2 center, float radius, Color color, int segments = 16);
  void DrawWireCircle(Vector2 center, float radius, Color color, int segments = 16);

  void DrawSolidArc(Vector2 center, float startAngle, float endAngle, float radius, Color color, int segments = 16);
  void DrawWireArc(Vector2 center, float startAngle, float endAngle, float radius, Color color, int segments = 16);

  void DrawWireCurve<TCurve>(TCurve curve, Color color, int resolution) where TCurve : IPlanarCurve;
}
