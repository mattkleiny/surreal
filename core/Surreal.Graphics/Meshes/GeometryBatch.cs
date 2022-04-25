using System.Runtime.InteropServices;
using Surreal.Graphics.Shaders;
using Surreal.Mathematics;
using Surreal.Memory;
using static Surreal.Mathematics.Maths;

namespace Surreal.Graphics.Meshes;

/// <summary>An efficient batch of geometric primitives for rendering to the GPU.</summary>
public sealed class GeometryBatch : IDisposable
{
  private const int DefaultVertexCount = 16_000;

  private readonly IDisposableBuffer<Vertex> vertices;
  private readonly Mesh<Vertex> mesh;

  private ShaderProgram? shader;
  private int vertexCount;

  public GeometryBatch(IGraphicsServer server, int maximumVertexCount = DefaultVertexCount)
  {
    vertices = Buffers.AllocateNative<Vertex>(maximumVertexCount);
    mesh     = new Mesh<Vertex>(server);
  }

  public void Begin(ShaderProgram shader)
    => this.shader = shader;

  public void DrawPoint(Vector2 position, Color color)
    => DrawPrimitive(stackalloc[] { position }, color, MeshType.Points);

  public void DrawLine(Vector2 from, Vector2 to, Color color)
    => DrawPrimitive(stackalloc[] { from, to }, color, MeshType.Lines);

  public void DrawLines(ReadOnlySpan<Vector2> points, Color color)
    => DrawPrimitive(points, color, MeshType.Lines);

  public void DrawLineLoop(ReadOnlySpan<Vector2> points, Color color)
    => DrawPrimitive(points, color, MeshType.LineLoop);

  public void DrawLineStrip(ReadOnlySpan<Vector2> points, Color color)
    => DrawPrimitive(points, color, MeshType.LineStrip);

  public void DrawSolidTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
    => DrawPrimitive(stackalloc[] { a, b, c }, color, MeshType.Triangles);

  public void DrawWireTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
    => DrawPrimitive(stackalloc[] { a, b, c }, color, MeshType.LineLoop);

  public void DrawSolidQuad(Vector2 center, Vector2 size, Color color)
    => DrawQuad(center, size, color, MeshType.Triangles);

  public void DrawWireQuad(Vector2 center, Vector2 size, Color color)
    => DrawQuad(center, size, color, MeshType.LineLoop);

  public void DrawCircle(Vector2 center, float radius, Color color, int segments = 16)
  {
    var points = new SpanList<Vector2>(stackalloc Vector2[segments]);
    var increment = 360f / segments;

    for (var theta = 0f; theta < 360f; theta += increment)
    {
      var x = radius * MathF.Cos(DegreesToRadians(theta)) + center.X;
      var y = radius * MathF.Sin(DegreesToRadians(theta)) + center.Y;

      points.Add(new Vector2(x, y));
    }

    DrawLineLoop(points, color);
  }

  public void DrawArc(Vector2 center, float startAngle, float endAngle, float radius, Color color, int segments = 16)
  {
    var points = new SpanList<Vector2>(stackalloc Vector2[segments]);
    var length = endAngle - startAngle;
    var increment = length / segments;

    for (var theta = startAngle; theta < endAngle; theta += increment)
    {
      var x = radius * MathF.Cos(DegreesToRadians(theta)) + center.X;
      var y = radius * MathF.Sin(DegreesToRadians(theta)) + center.Y;

      points.Add(new Vector2(x, y));
    }

    DrawLineStrip(points, color);
  }

  public void DrawCurve<TCurve>(TCurve curve, Color color, int resolution)
    where TCurve : IPlanarCurve
  {
    var points = new SpanList<Vector2>(stackalloc Vector2[resolution]);

    for (var i = 0; i < resolution; i++)
    {
      var x = (float) i / resolution;

      points.Add(curve.SampleAt(x));
    }

    DrawLineStrip(points.ToSpan(), color);
  }

  private void DrawQuad(Vector2 center, Vector2 size, Color color, MeshType type)
  {
    var halfWidth = size.X / 2f;
    var halfHeight = size.Y / 2f;

    var bottomLeft = new Vector2(center.X - halfWidth, center.Y - halfHeight);
    var topLeft = new Vector2(center.X - halfWidth, center.Y + halfHeight);
    var topRight = new Vector2(center.X + halfWidth, center.Y + halfHeight);
    var bottomRight = new Vector2(center.X + halfWidth, center.Y - halfHeight);

    DrawPrimitive(
      points: stackalloc Vector2[]
      {
        // triangle 1
        bottomLeft,
        topLeft,
        topRight,

        // triangle 2
        topRight,
        bottomRight,
        bottomLeft,
      },
      color: color,
      type: type
    );
  }

  public void DrawPrimitive(ReadOnlySpan<Vector2> points, Color color, MeshType type)
  {
    var destination = new SpanList<Vertex>(vertices.Data.Span[vertexCount..points.Length]);

    for (var i = 0; i < points.Length; i++)
    {
      destination.Add(new Vertex(points[i], color));
    }

    vertexCount += points.Length;

    Flush(type);
  }

  private void Flush(MeshType type)
  {
    if (vertexCount == 0) return;
    if (shader == null) return;

    mesh.Vertices.Write(vertices.Data.Span[..vertexCount]);
    mesh.Draw(shader, vertexCount, 0, type);

    vertexCount = 0;
  }

  public void Dispose()
  {
    mesh.Dispose();
    vertices.Dispose();
  }

  /// <summary>A single vertex in the batch.</summary>
  [StructLayout(LayoutKind.Sequential)]
  private record struct Vertex(Vector2 Position, Color Color)
  {
    [VertexDescriptor(
      Alias = "in_position",
      Count = 2,
      Type = VertexType.Float
    )]
    public Vector2 Position = Position;

    [VertexDescriptor(
      Alias = "in_color",
      Count = 4,
      Type = VertexType.Float
    )]
    public Color Color = Color;
  }
}
