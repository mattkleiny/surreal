using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Surreal.Collections;
using Surreal.Graphics.Materials.Shaders;
using Surreal.IO;
using Surreal.Mathematics.Curves;
using Surreal.Mathematics.Linear;
using static Surreal.Mathematics.Maths;

namespace Surreal.Graphics.Meshes {
  // TODO: support poly lines and normals for thick lines and animated edges
  // TODO: support arbitrary transforms for wireframe geometry?

  public sealed class GeometryBatch : IDisposable {
    private const string ProjViewUniform = "u_projView";

    private readonly IDisposableBuffer<Vertex> vertices;
    private readonly IDisposableBuffer<ushort> indices;
    private readonly Mesh<Vertex>              mesh;
    private readonly ShaderProgram             defaultShader;
    private readonly bool                      ownsDefaultShader;

    private int vertexCount;
    private int indexCount;

    public static GeometryBatch Create(IGraphicsDevice device, ShaderProgram shader) {
      return new GeometryBatch(device, shader, ownsDefaultShader: false);
    }

    public static async Task<GeometryBatch> CreateDefaultAsync(IGraphicsDevice device) {
      var shader = device.CreateShaderProgram(
          await Shader.LoadAsync(ShaderType.Vertex, "resx://Surreal.Graphics/Resources/Shaders/GeometryBatch.vert.glsl"),
          await Shader.LoadAsync(ShaderType.Fragment, "resx://Surreal.Graphics/Resources/Shaders/GeometryBatch.frag.glsl")
      );

      return new GeometryBatch(device, shader, ownsDefaultShader: true);
    }

    private GeometryBatch(
        IGraphicsDevice device,
        ShaderProgram defaultShader,
        bool ownsDefaultShader,
        int maximumVertexCount = 16_000,
        int maximumIndexCount = 16_000 * 6) {
      Device = device;

      this.defaultShader     = defaultShader;
      this.ownsDefaultShader = ownsDefaultShader;

      vertices = Buffers.AllocateOffHeap<Vertex>(maximumVertexCount);
      indices  = Buffers.AllocateOffHeap<ushort>(maximumIndexCount);

      mesh = new Mesh<Vertex>(device);
    }

    public IGraphicsDevice Device       { get; }
    public ShaderProgram?  ActiveShader { get; private set; }

    public void Begin(in Matrix4x4 projectionView) {
      Begin(defaultShader, in projectionView);
    }

    public void Begin(ShaderProgram shader, in Matrix4x4 projectionView) {
      shader.SetUniform(ProjViewUniform, in projectionView);

      ActiveShader = shader;
    }

    public void DrawPoint(Vector2 position, Color color)
      => DrawPrimitive(stackalloc[] {position}, color, PrimitiveType.Points);

    public void DrawLine(Vector2 from, Vector2 to, Color color)
      => DrawPrimitive(stackalloc[] {from, to}, color, PrimitiveType.Lines);

    public void DrawLine(Line segment, Color color)
      => DrawPrimitive(stackalloc[] {segment.From, segment.To}, color, PrimitiveType.Lines);

    public void DrawLines(ReadOnlySpan<Vector2> points, Color color)
      => DrawPrimitive(points, color, PrimitiveType.Lines);

    public void DrawLineLoop(ReadOnlySpan<Vector2> points, Color color)
      => DrawPrimitive(points, color, PrimitiveType.LineLoop);

    public void DrawLineStrip(ReadOnlySpan<Vector2> points, Color color)
      => DrawPrimitive(points, color, PrimitiveType.LineStrip);

    public void DrawSolidTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
      => DrawPrimitive(stackalloc[] {a, b, c}, color, PrimitiveType.Triangles);

    public void DrawWireTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
      => DrawPrimitive(stackalloc[] {a, b, c}, color, PrimitiveType.LineLoop);

    public void DrawSolidQuad(Vector2 center, Vector2 size, Color color)
      => DrawQuad(center, size, color, PrimitiveType.Quads);

    public void DrawWireQuad(Vector2 center, Vector2 size, Color color)
      => DrawQuad(center, size, color, PrimitiveType.LineLoop);

    [SkipLocalsInit]
    public unsafe void DrawCircle(Vector2 center, float radius, Color color, int segments = 16) {
      var points    = new SpanList<Vector2>(stackalloc Vector2[segments]);
      var increment = 360f / segments;

      for (var theta = 0f; theta < 360f; theta += increment) {
        var x = radius * MathF.Cos(DegreesToRadians(theta)) + center.X;
        var y = radius * MathF.Sin(DegreesToRadians(theta)) + center.Y;

        points.Add(new Vector2(x, y));
      }

      DrawLineLoop(points.Span, color);
    }

    [SkipLocalsInit]
    public unsafe void DrawArc(Vector2 center, float startAngle, float endAngle, float radius, Color color, int segments = 16) {
      var points    = new SpanList<Vector2>(stackalloc Vector2[segments]);
      var length    = endAngle - startAngle;
      var increment = length / segments;

      for (var theta = startAngle; theta < endAngle; theta += increment) {
        var x = radius * MathF.Cos(DegreesToRadians(theta)) + center.X;
        var y = radius * MathF.Sin(DegreesToRadians(theta)) + center.Y;

        points.Add(new Vector2(x, y));
      }

      DrawLineStrip(points.Span, color);
    }

    [SkipLocalsInit]
    public unsafe void DrawCurve<TCurve>(TCurve curve, Color color, int resolution)
        where TCurve : IPlanarCurve {
      var points = new SpanList<Vector2>(stackalloc Vector2[resolution]);

      for (var i = 0; i < resolution; i++) {
        var x = (float) i / resolution;

        points.Add(curve.SampleAt(x));
      }

      DrawLineStrip(points.Span, color);
    }

    [SkipLocalsInit]
    public unsafe void DrawQuad(Vector2 center, Vector2 size, Color color, PrimitiveType type) {
      var halfWidth  = size.X / 2f;
      var halfHeight = size.Y / 2f;

      DrawPrimitive(
          points: stackalloc Vector2[] {
              new Vector2(center.X - halfWidth, center.Y - halfHeight),
              new Vector2(center.X - halfWidth, center.Y + halfHeight),
              new Vector2(center.X + halfWidth, center.Y + halfHeight),
              new Vector2(center.X + halfWidth, center.Y - halfHeight),
          },
          color: color,
          type: type
      );
    }

    [SkipLocalsInit]
    public unsafe void DrawPrimitive(ReadOnlySpan<Vector2> points, Color color, PrimitiveType type) {
      var destination = new SpanList<Vertex>(vertices.Span[vertexCount..points.Length]);

      for (var i = 0; i < points.Length; i++) {
        destination.Add(new Vertex(points[i], color));
      }

      vertexCount += points.Length;

      Flush(type);
    }

    public void End() {
    }

    private void Flush(PrimitiveType type) {
      if (vertexCount == 0) return;

      mesh.Vertices.Write(vertices.Span[..vertexCount]);
      mesh.Indices.Write(indices.Span[..indexCount]);

      mesh.DrawImmediate(ActiveShader!, vertexCount, indexCount, type);

      vertexCount = 0;
      indexCount  = 0;
    }

    public void Dispose() {
      if (ownsDefaultShader) {
        defaultShader.Dispose();
      }

      mesh.Dispose();

      vertices.Dispose();
      indices.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Vertex {
      [VertexAttribute(
          Alias = "a_position",
          Count = 2,
          Type  = VertexType.Float
      )]
      public Vector2 Position;

      [VertexAttribute(
          Alias      = "a_color",
          Count      = 4,
          Type       = VertexType.UnsignedByte,
          Normalized = true
      )]
      public Color Color;

      public Vertex(Vector2 position, Color color) {
        Position = position;
        Color    = color;
      }
    }
  }
}