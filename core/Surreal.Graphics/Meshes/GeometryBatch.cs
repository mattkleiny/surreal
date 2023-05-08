using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Shaders;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

/// <summary>
/// An efficient batch of geometric primitives for rendering to the GPU.
/// </summary>
public sealed class GeometryBatch : IDisposable
{
  private const int DefaultVertexCount = 64;
  private readonly Mesh<Vertex2> _mesh;

  private readonly IDisposableBuffer<Vertex2> _vertices;

  private Material? _material;
  private int _vertexCount;

  public GeometryBatch(IGraphicsServer server, int maximumVertexCount = DefaultVertexCount)
  {
    _vertices = Buffers.AllocateNative<Vertex2>(maximumVertexCount);
    _mesh = new Mesh<Vertex2>(server);
  }

  public void Dispose()
  {
    _mesh.Dispose();
    _vertices.Dispose();
  }

  public void Begin(ShaderProgram shader)
  {
    _material = new Material(shader);
  }

  public void Begin(Material material)
  {
    _vertexCount = 0; // reset vertex pointer

    _material = material;
  }

  public void DrawPoint(Vector2 position, ColorF color)
  {
    DrawPrimitive(stackalloc[] { position }, color, MeshType.Points);
  }

  public void DrawLine(Vector2 from, Vector2 to, ColorF color)
  {
    DrawPrimitive(stackalloc[] { from, to }, color, MeshType.Lines);
  }

  public void DrawLines(ReadOnlySpan<Vector2> points, ColorF color)
  {
    DrawPrimitive(points, color, MeshType.Lines);
  }

  public void DrawLineLoop(ReadOnlySpan<Vector2> points, ColorF color)
  {
    DrawPrimitive(points, color, MeshType.LineLoop);
  }

  public void DrawLineStrip(ReadOnlySpan<Vector2> points, ColorF color)
  {
    DrawPrimitive(points, color, MeshType.LineStrip);
  }

  public void DrawSolidTriangle(Vector2 a, Vector2 b, Vector2 c, ColorF color)
  {
    DrawPrimitive(stackalloc[] { a, b, c }, color, MeshType.Triangles);
  }

  public void DrawWireTriangle(Vector2 a, Vector2 b, Vector2 c, ColorF color)
  {
    DrawPrimitive(stackalloc[] { a, b, c }, color, MeshType.LineLoop);
  }

  public void DrawSolidQuad(Rectangle rectangle, ColorF color)
  {
    DrawQuad(rectangle.Center, rectangle.Size, color, MeshType.Triangles);
  }

  public void DrawSolidQuad(Vector2 center, Vector2 size, ColorF color)
  {
    DrawQuad(center, size, color, MeshType.Triangles);
  }

  public void DrawWireQuad(Rectangle rectangle, ColorF color)
  {
    DrawQuad(rectangle.Center, rectangle.Size, color, MeshType.LineLoop);
  }

  public void DrawWireQuad(Vector2 center, Vector2 size, ColorF color)
  {
    DrawQuad(center, size, color, MeshType.LineLoop);
  }

  public void DrawCircle(Vector2 center, float radius, ColorF color, int segments = 16)
  {
    var points = new SpanList<Vector2>(stackalloc Vector2[segments]);
    var increment = 360f / segments;

    for (var theta = 0f; theta < 360f; theta += increment)
    {
      var x = radius * MathF.Cos(Maths.DegreesToRadians(theta)) + center.X;
      var y = radius * MathF.Sin(Maths.DegreesToRadians(theta)) + center.Y;

      points.Add(new Vector2(x, y));
    }

    DrawLineLoop(points, color);
  }

  public void DrawArc(Vector2 center, float startAngle, float endAngle, float radius, ColorF color, int segments = 16)
  {
    var points = new SpanList<Vector2>(stackalloc Vector2[segments]);
    var length = endAngle - startAngle;
    var increment = length / segments;

    for (var theta = startAngle; theta < endAngle; theta += increment)
    {
      var x = radius * MathF.Cos(Maths.DegreesToRadians(theta)) + center.X;
      var y = radius * MathF.Sin(Maths.DegreesToRadians(theta)) + center.Y;

      points.Add(new Vector2(x, y));
    }

    DrawLineStrip(points, color);
  }

  public void DrawCurve<TCurve>(TCurve curve, ColorF color, int resolution)
    where TCurve : IPlanarCurve
  {
    var points = new SpanList<Vector2>(stackalloc Vector2[resolution]);

    for (var i = 0; i < resolution; i++)
    {
      var x = (float)i / resolution;

      points.Add(curve.SampleAt(x));
    }

    DrawLineStrip(points.ToSpan(), color);
  }

  private void DrawQuad(Vector2 center, Vector2 size, ColorF color, MeshType type)
  {
    var halfWidth = size.X / 2f;
    var halfHeight = size.Y / 2f;

    var bottomLeft = new Vector2(center.X - halfWidth, center.Y - halfHeight);
    var topLeft = new Vector2(center.X - halfWidth, center.Y + halfHeight);
    var topRight = new Vector2(center.X + halfWidth, center.Y + halfHeight);
    var bottomRight = new Vector2(center.X + halfWidth, center.Y - halfHeight);

    DrawPrimitive(
      stackalloc Vector2[]
      {
        // triangle 1
        bottomLeft,
        topLeft,
        topRight,

        // triangle 2
        topRight,
        bottomRight,
        bottomLeft
      },
      color,
      type
    );
  }

  public void DrawPrimitive(ReadOnlySpan<Vector2> points, ColorF color, MeshType type)
  {
    var destination = new SpanList<Vertex2>(_vertices.Span[_vertexCount..points.Length]);

    for (var i = 0; i < points.Length; i++) destination.Add(new Vertex2(points[i], color, Vector2.Zero));

    _vertexCount += points.Length;

    Flush(type);
  }

  private void Flush(MeshType type)
  {
    if (_vertexCount == 0)
    {
      return;
    }

    if (_material == null)
    {
      return;
    }

    _mesh.Vertices.Write(_vertices.Span[.._vertexCount]);
    _mesh.Draw(_material, _vertexCount, 0, type);

    _vertexCount = 0;
  }
}
