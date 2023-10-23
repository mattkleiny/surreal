using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Maths;

namespace Surreal.Graphics.Gizmos;

/// <summary>
/// A batch for rendering gizmos.
/// </summary>
public sealed class GizmoBatch(IGraphicsBackend backend) : IDisposable
{
  private readonly GeometryBatch _geometryBatch = new(backend);

  public void Begin(Material material)
  {
    _geometryBatch.Begin(material);
  }

  public void DrawPoint(Vector2 position, Color color)
  {
    _geometryBatch.DrawPoint(position, color);
  }

  public void DrawLine(Vector2 from, Vector2 to, Color color)
  {
    _geometryBatch.DrawLine(from, to, color);
  }

  public void DrawLines(ReadOnlySpan<Vector2> points, Color color)
  {
    _geometryBatch.DrawLines(points, color);
  }

  public void DrawLineLoop(ReadOnlySpan<Vector2> points, Color color)
  {
    _geometryBatch.DrawLineLoop(points, color);
  }

  public void DrawLineStrip(ReadOnlySpan<Vector2> points, Color color)
  {
    _geometryBatch.DrawLineStrip(points, color);
  }

  public void DrawSolidTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
  {
    _geometryBatch.DrawSolidTriangle(a, b, c, color);
  }

  public void DrawWireTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
  {
    _geometryBatch.DrawWireTriangle(a, b, c, color);
  }

  public void DrawSolidQuad(Rectangle rectangle, Color color)
  {
    _geometryBatch.DrawSolidQuad(rectangle, color);
  }

  public void DrawSolidQuad(Vector2 center, Vector2 size, Color color)
  {
    _geometryBatch.DrawSolidQuad(center, size, color);
  }

  public void DrawWireQuad(Rectangle rectangle, Color color)
  {
    _geometryBatch.DrawWireQuad(rectangle, color);
  }

  public void DrawWireQuad(Vector2 center, Vector2 size, Color color)
  {
    _geometryBatch.DrawWireQuad(center, size, color);
  }

  public void DrawWireCircle(Vector2 center, float radius, Color color, int segments = 16)
  {
    _geometryBatch.DrawWireCircle(center, radius, color, segments);
  }

  public void DrawWireArc(Vector2 center, float startAngle, float endAngle, float radius, Color color, int segments = 16)
  {
    _geometryBatch.DrawWireArc(center, startAngle, endAngle, radius, color, segments);
  }

  public void DrawWireCurve<TCurve>(TCurve curve, Color color, int resolution) where TCurve : IPlanarCurve
  {
    _geometryBatch.DrawWireCurve(curve, color, resolution);
  }

  public void Flush()
  {
    _geometryBatch.Flush();
  }

  public void Dispose()
  {
    _geometryBatch.Dispose();
  }
}
