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

  private static readonly MaterialProperty<Matrix4x4> ProjectionView = new("_ProjectionView");

  private readonly IDisposableBuffer<Vertex> vertices;
  private readonly IDisposableBuffer<ushort> indices;
  private readonly Mesh<Vertex>              mesh;

  private Material? material;
  private int       vertexCount;
  private int       indexCount;

  public GeometryBatch(
    IGraphicsServer server,
    int maximumVertexCount = DefaultVertexCount,
    int maximumIndexCount = DefaultVertexCount * 6
  )
  {
    vertices = Buffers.AllocateNative<Vertex>(maximumVertexCount);
    indices  = Buffers.AllocateNative<ushort>(maximumIndexCount);

    mesh = new Mesh<Vertex>(server);
  }

  public void Begin(Material material, in Matrix4x4 projectionView)
  {
    this.material = material;

    // TODO: set projection view?
    // material.SetProperty(ProjectionView, in projectionView);
  }

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
    => DrawQuad(center, size, color, MeshType.Quads);

  public void DrawWireQuad(Vector2 center, Vector2 size, Color color)
    => DrawQuad(center, size, color, MeshType.LineLoop);

  public void DrawCircle(Vector2 center, float radius, Color color, int segments = 16)
  {
    var points    = new SpanList<Vector2>(stackalloc Vector2[segments]);
    var increment = 360f / segments;

    for (var theta = 0f; theta < 360f; theta += increment)
    {
      var x = radius * MathF.Cos(DegreesToRadians(theta)) + center.X;
      var y = radius * MathF.Sin(DegreesToRadians(theta)) + center.Y;

      points.Add(new Vector2(x, y));
    }

    DrawLineLoop(points.ToSpan(), color);
  }

  public void DrawArc(Vector2 center, float startAngle, float endAngle, float radius, Color color, int segments = 16)
  {
    var points    = new SpanList<Vector2>(stackalloc Vector2[segments]);
    var length    = endAngle - startAngle;
    var increment = length / segments;

    for (var theta = startAngle; theta < endAngle; theta += increment)
    {
      var x = radius * MathF.Cos(DegreesToRadians(theta)) + center.X;
      var y = radius * MathF.Sin(DegreesToRadians(theta)) + center.Y;

      points.Add(new Vector2(x, y));
    }

    DrawLineStrip(points.ToSpan(), color);
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

  public void DrawQuad(Vector2 center, Vector2 size, Color color, MeshType type)
  {
    var halfWidth  = size.X / 2f;
    var halfHeight = size.Y / 2f;

    DrawPrimitive(
      points: stackalloc Vector2[]
      {
        new Vector2(center.X - halfWidth, center.Y - halfHeight),
        new Vector2(center.X - halfWidth, center.Y + halfHeight),
        new Vector2(center.X + halfWidth, center.Y + halfHeight),
        new Vector2(center.X + halfWidth, center.Y - halfHeight),
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
    if (material == null) return;

    mesh.Vertices.Write(vertices.Data.Span[..vertexCount]);
    mesh.Indices.Write(indices.Data.Span[..indexCount]);

    mesh.DrawImmediate(material, vertexCount, indexCount, type);

    vertexCount = 0;
    indexCount  = 0;
  }

  public void Dispose()
  {
    mesh.Dispose();

    vertices.Dispose();
    indices.Dispose();
  }

  /// <summary>A single vertex in the batch.</summary>
  [VisibleForTesting]
  [StructLayout(LayoutKind.Sequential)]
  internal record struct Vertex(Vector2 Position, Color Color)
  {
    [VertexDescriptor(
      Alias = "a_position",
      Count = 2,
      Type = VertexType.Float
    )]
    public Vector2 Position = Position;

    [VertexDescriptor(
      Alias = "a_color",
      Count = 4,
      Type = VertexType.UnsignedByte,
      Normalized = true
    )]
    public Color Color = Color;
  }
}
