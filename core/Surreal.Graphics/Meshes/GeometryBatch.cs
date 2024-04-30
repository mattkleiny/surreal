using Surreal.Colors;
using Surreal.Diagnostics.Gizmos;
using Surreal.Graphics.Materials;
using Surreal.Maths;
using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

/// <summary>
/// An efficient batch of geometric primitives for rendering to the GPU.
/// </summary>
public sealed class GeometryBatch(IGraphicsBackend backend, int maximumVertexCount = GeometryBatch.DefaultVertexCount) : IGizmoBatch, IDisposable
{
  private const int DefaultVertexCount = 512;

  private readonly Mesh<Vertex2> _mesh = new(backend);
  private readonly IDisposableBuffer<Vertex2> _vertices = Buffers.AllocateNative<Vertex2>(maximumVertexCount);
  private readonly IDisposableBuffer<uint> _indices = Buffers.AllocateNative<uint>((maximumVertexCount - 1) * 3);

  private Material? _material;
  private int _vertexCount;
  private int _indexCount;
  private PolygonMode _lastPolygonMode;

  /// <summary>
  /// The material to use for rendering the batch.
  /// </summary>
  public Material? Material
  {
    get => _material;
    set
    {
      if (_material != value)
      {
        Flush();
        _material = value;
      }
    }
  }

  /// <summary>
  /// Draws the given gizmo object.
  /// </summary>
  public void DrawObject(IGizmoObject gizmoObject)
    => gizmoObject.RenderGizmos(this);

  /// <summary>
  /// Draws a point at the given position.
  /// </summary>
  public void DrawPoint(Vector2 position, Color color)
    => DrawTriangleFan([position], color, PolygonMode.Lines);

  /// <summary>
  /// Draws a line from the given start to end points.
  /// </summary>
  public void DrawLine(Vector2 from, Vector2 to, Color color)
    => DrawWireTriangle(from, from, to, color);

  /// <summary>
  /// Draws a strip of lines from the given points.
  /// </summary>
  public void DrawLineStrip(SpanList<Vector2> points, Color color)
  {
    for (var i = 0; i < points.Count - 1; i ++)
    {
      var a = points[i + 0];
      var b = points[i + 1];

      DrawLine(a, b, color);
    }

    // connect last segment
    if (points.Count > 2)
    {
      var a = points[^1];
      var b = points[0];

      DrawLine(a, b, color);
    }
  }

  /// <summary>
  /// Draws a solid triangle.
  /// </summary>
  public void DrawSolidTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
    => DrawTriangle(a, b, c, color, PolygonMode.Filled);

  /// <summary>
  /// Draws wireframe triangle.
  /// </summary>
  public void DrawWireTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
    => DrawTriangle(a, b, c, color, PolygonMode.Lines);

  private void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color, PolygonMode type)
    => DrawTriangleFan([a, b, c], color, type);

  /// <summary>
  /// Draws a solid quad.
  /// </summary>
  public void DrawSolidQuad(Rectangle rectangle, Color color)
    => DrawQuad(rectangle.Center, rectangle.Size, color, PolygonMode.Filled);

  /// <summary>
  /// Draws a wireframe quad.
  /// </summary>
  public void DrawWireQuad(Rectangle rectangle, Color color)
    => DrawQuad(rectangle.Center, rectangle.Size, color, PolygonMode.Lines);

  /// <summary>
  /// Draws a solid quad.
  /// </summary>
  public void DrawSolidQuad(Vector2 center, Vector2 size, Color color)
    => DrawQuad(center, size, color, PolygonMode.Filled);

  /// <summary>
  /// Draws a wireframe quad.
  /// </summary>
  public void DrawWireQuad(Vector2 center, Vector2 size, Color color)
    => DrawQuad(center, size, color, PolygonMode.Lines);

  private void DrawQuad(Vector2 center, Vector2 size, Color color, PolygonMode type)
  {
    var halfWidth = size.X / 2f;
    var halfHeight = size.Y / 2f;

    var bottomLeft = new Vector2(center.X - halfWidth, center.Y - halfHeight);
    var topLeft = new Vector2(center.X - halfWidth, center.Y + halfHeight);
    var topRight = new Vector2(center.X + halfWidth, center.Y + halfHeight);
    var bottomRight = new Vector2(center.X + halfWidth, center.Y - halfHeight);

    DrawTriangle(bottomLeft, topLeft, topRight, color, type);
    DrawTriangle(topRight, bottomRight, bottomLeft, color, type);
  }

  /// <summary>
  /// Draws a solid circle.
  /// </summary>
  public void DrawSolidCircle(Vector2 center, float radius, Color color, int segments)
  {
    var points = new SpanList<Vector2>(stackalloc Vector2[segments]);
    var increment = 360f / segments;

    for (var theta = 0f; theta < 360f; theta += increment)
    {
      var x = radius * MathF.Cos(MathE.DegreesToRadians(theta)) + center.X;
      var y = radius * MathF.Sin(MathE.DegreesToRadians(theta)) + center.Y;

      points.Add(new Vector2(x, y));
    }

    DrawTriangleFan(points, color, PolygonMode.Filled);
  }

  /// <summary>
  /// Draws a wireframe circle.
  /// </summary>
  public void DrawWireCircle(Vector2 center, float radius, Color color, int segments = 16)
  {
    var points = new SpanList<Vector2>(stackalloc Vector2[segments]);
    var increment = 360f / segments;

    for (var theta = 0f; theta < 360f; theta += increment)
    {
      var x = radius * MathF.Cos(MathE.DegreesToRadians(theta)) + center.X;
      var y = radius * MathF.Sin(MathE.DegreesToRadians(theta)) + center.Y;

      points.Add(new Vector2(x, y));
    }

    DrawLineStrip(points, color);
  }

  /// <summary>
  /// Draws a solid arc.
  /// </summary>
  public void DrawSolidArc(Vector2 center, float startAngle, float endAngle, float radius, Color color, int segments = 16)
    => DrawArc(center, startAngle, endAngle, radius, color, segments, PolygonMode.Filled);

  /// <summary>
  /// Draws a wireframe arc.
  /// </summary>
  public void DrawWireArc(Vector2 center, float startAngle, float endAngle, float radius, Color color, int segments = 16)
    => DrawArc(center, startAngle, endAngle, radius, color, segments, PolygonMode.Lines);

  private void DrawArc(Vector2 center, float startAngle, float endAngle, float radius, Color color, int segments, PolygonMode mode)
  {
    var points = new SpanList<Vector2>(stackalloc Vector2[segments]);
    var length = endAngle - startAngle;
    var increment = length / segments;

    for (var theta = startAngle; theta < endAngle; theta += increment)
    {
      var x = radius * MathF.Cos(MathE.DegreesToRadians(theta)) + center.X;
      var y = radius * MathF.Sin(MathE.DegreesToRadians(theta)) + center.Y;

      points.Add(new Vector2(x, y));
    }

    DrawTriangleFan(points, color, mode);
  }

  /// <summary>
  /// Draws a wireframe curve.
  /// </summary>
  public void DrawWireCurve<TCurve>(TCurve curve, Color color, int resolution)
    where TCurve : IPlanarCurve
  {
    var points = new SpanList<Vector2>(stackalloc Vector2[resolution]);

    for (var i = 0; i < resolution; i++)
    {
      var x = (float)i / resolution;

      points.Add(curve.SampleAt(x));
    }

    DrawTriangleFan(points, color, PolygonMode.Lines);
  }

  private void DrawTriangleFan(ReadOnlySpan<Vector2> points, Color color, PolygonMode mode)
  {
    var finalVertexCount = (points.Length - 2) * 2 + 1;
    var finalIndexCount = (points.Length - 2) * 3;

    if (_lastPolygonMode != mode)
    {
      // if we're switching geometry types, we'll need to flush and start again
      Flush();
      _lastPolygonMode = mode;
    }
    else if (_vertexCount + finalVertexCount > _vertices.Span.Length ||
             _indexCount + finalIndexCount > _indices.Span.Length)
    {
      // if we've exceeded the batch capacity, we'll need to flush and start again
      Flush();
    }

    var vertices = new SpanList<Vertex2>(_vertices.Span[_vertexCount..]);
    var indices = new SpanList<uint>(_indices.Span[_indexCount..]);

    vertices.Add(new Vertex2(points[0], color, Vector2.Zero));

    var startIndex = (uint)_vertexCount;

    for (var i = 1; i < points.Length - 1; i++)
    {
      var offset = startIndex + vertices.Count;

      vertices.Add(new Vertex2(points[i + 0], color, Vector2.Zero));
      vertices.Add(new Vertex2(points[i + 1], color, Vector2.Zero));

      indices.Add(startIndex);
      indices.Add((uint)(offset + 0));
      indices.Add((uint)(offset + 1));
    }

    _vertexCount += vertices.Count;
    _indexCount += indices.Count;
  }

  /// <summary>
  /// Resets the batch, discarding any pending geometry.
  /// </summary>
  public void Reset()
  {
    _vertexCount = 0;
    _indexCount = 0;
  }

  /// <summary>
  /// Flushes the batch to the GPU.
  /// </summary>
  public void Flush()
  {
    if (_vertexCount == 0) return;

    if (_material != null)
    {
      _material.PolygonMode = _lastPolygonMode;

      _mesh.Vertices.Write(_vertices.Span[.._vertexCount]);
      _mesh.Indices.Write(_indices.Span[.._indexCount]);

      _mesh.Draw(_material);
    }

    Reset();
  }

  public void Dispose()
  {
    _mesh.Dispose();
    _vertices.Dispose();
    _indices.Dispose();
  }
}
