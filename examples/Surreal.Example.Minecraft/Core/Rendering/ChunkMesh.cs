using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Surreal.Collections;
using Surreal.Diagnostics.Profiling;
using Surreal.Framework;
using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.IO;
using static Surreal.Mathematics.Maths;

namespace Minecraft.Core.Rendering {
  public sealed class ChunkMesh : IDisposable {
    private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<ChunkMesh>();

    private readonly Chunk chunk;
    private readonly Mesh  mesh;

    public ChunkMesh(IGraphicsDevice device, Chunk chunk) {
      this.chunk = chunk;

      mesh = Mesh.Create<Vertex>(device);

      chunk.BlockChanged += OnBlockChanged;
    }

    private void OnBlockChanged(int x, int y, int z, Block oldBlock, Block newBlock) => IsDirty = true;

    public bool IsDirty { get; private set; } = true;
    public bool IsReady { get; private set; }

    public void Render(ShaderProgram program, PrimitiveType type) {
      if (IsDirty) {
        IsDirty = false;
        IsReady = false;

        Invalidate();
      }

      if (IsReady) {
        mesh.DrawImmediate(program, type);
      }
    }

    private void Invalidate() => Task.Run(() => {
      using (Profiler.Track(nameof(Invalidate))) {
        var tessellator = new Tessellator();

        for (var z = 0; z < chunk.Depth; z++)
        for (var y = 0; y < chunk.Height; y++)
        for (var x = 0; x < chunk.Width; x++) {
          var block = chunk[x, y, z];
          if (!block.IsSolid) continue; // don't render geometry for non-solid blocks

          if (!chunk[x - 1, y, z].IsSolid) tessellator.AddFace(x, y, z, Face.Left, block.Color);
          if (!chunk[x + 1, y, z].IsSolid) tessellator.AddFace(x, y, z, Face.Right, block.Color);
          if (!chunk[x, y + 1, z].IsSolid) tessellator.AddFace(x, y, z, Face.Top, block.Color);
          if (!chunk[x, y - 1, z].IsSolid) tessellator.AddFace(x, y, z, Face.Bottom, block.Color);
          if (!chunk[x, y, z - 1].IsSolid) tessellator.AddFace(x, y, z, Face.Front, block.Color);
          if (!chunk[x, y, z + 1].IsSolid) tessellator.AddFace(x, y, z, Face.Back, block.Color);
        }

        Engine.Schedule(() => {
          using var _ = tessellator; // make sure to clean up tessellator resources

          // upload vertices/indices to the GPU
          mesh.Vertices.Write(tessellator.Vertices);
          mesh.Indices.Write(tessellator.Indices);

          IsReady = true;
        });
      }
    });

    public void Dispose() {
      chunk.BlockChanged -= OnBlockChanged;

      mesh.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Vertex {
      [VertexAttribute(
          Alias = "a_position",
          Count = 3,
          Type  = VertexType.Float
      )]
      public Vector3 Position;

      [VertexAttribute(
          Alias = "a_normal",
          Count = 3,
          Type  = VertexType.Float
      )]
      public Vector3 Normal;

      [VertexAttribute(
          Alias      = "a_color",
          Count      = 4,
          Type       = VertexType.UnsignedByte,
          Normalized = true
      )]
      public Color Color;

      [VertexAttribute(
          Alias      = "a_texCoord0",
          Count      = 2,
          Type       = VertexType.Float,
          Normalized = true
      )]
      public UV UV;

      public Vertex(Vector3 position, Vector3 normal, Color color, UV uv) {
        Position = position;
        Normal   = normal;
        Color    = color;
        UV       = uv;
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct UV {
      [UsedImplicitly] public float U;
      [UsedImplicitly] public float V;

      public UV(float u, float v) {
        U = u;
        V = v;
      }
    }

    private sealed class Tessellator : IDisposable {
      private static readonly RingBuffer<int> VertexCountSamples = new RingBuffer<int>(30) {16_000}; // add a sane default to try and lower initial resize overhead

      private static int AverageVertexCount => VertexCountSamples.Sum() / Math.Max(1, VertexCountSamples.Count);
      private static int AverageIndexCount  => AverageVertexCount * 3 / 2;

      private readonly BufferList<Vertex> vertices;
      private readonly BufferList<ushort> indices;

      public Tessellator()
          : this(AverageVertexCount, AverageIndexCount) {
      }

      private Tessellator(int vertexCapacity, int indexCapacity) {
        vertices = new BufferList<Vertex>(Buffers.Allocate<Vertex>(vertexCapacity));
        indices  = new BufferList<ushort>(Buffers.Allocate<ushort>(indexCapacity));
      }

      public Span<Vertex> Vertices => vertices.Span;
      public Span<ushort> Indices  => indices.Span;

      public void AddFace(int x, int y, int z, Face face, Color color) {
        void AddIndices(ushort a, ushort b, ushort c) {
          indices.Add((ushort) (vertices.Count + a));
          indices.Add((ushort) (vertices.Count + b));
          indices.Add((ushort) (vertices.Count + c));
        }

        const float size = 0.5f;

        AddIndices(0, 1, 2);
        AddIndices(0, 2, 3);

        switch (face) {
          case Face.Left:
            vertices.Add(new Vertex(position: V(x - size, y - size, z + size), normal: V(-1f, 0f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x - size, y + size, z + size), normal: V(-1f, 0f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x - size, y + size, z - size), normal: V(-1f, 0f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x - size, y - size, z - size), normal: V(-1f, 0f, 0f), color, new UV(0f, 0f)));
            break;

          case Face.Right:
            vertices.Add(new Vertex(position: V(x + size, y - size, z - size), normal: V(1f, 0f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y + size, z - size), normal: V(1f, 0f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y + size, z + size), normal: V(1f, 0f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y - size, z + size), normal: V(1f, 0f, 0f), color, new UV(0f, 0f)));
            break;

          case Face.Top:
            vertices.Add(new Vertex(position: V(x - size, y + size, z + size), normal: V(0f, 1f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y + size, z + size), normal: V(0f, 1f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y + size, z - size), normal: V(0f, 1f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x - size, y + size, z - size), normal: V(0f, 1f, 0f), color, new UV(0f, 0f)));
            break;

          case Face.Bottom:
            vertices.Add(new Vertex(position: V(x - size, y - size, z + size), normal: V(0f, -1f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x - size, y - size, z - size), normal: V(0f, -1f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y - size, z - size), normal: V(0f, -1f, 0f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y - size, z + size), normal: V(0f, -1f, 0f), color, new UV(0f, 0f)));
            break;

          case Face.Front:
            vertices.Add(new Vertex(position: V(x - size, y - size, z - size), normal: V(0f, 0f, -1f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x - size, y + size, z - size), normal: V(0f, 0f, -1f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y + size, z - size), normal: V(0f, 0f, -1f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y - size, z - size), normal: V(0f, 0f, -1f), color, new UV(0f, 0f)));
            break;

          case Face.Back:
            vertices.Add(new Vertex(position: V(x - size, y + size, z + size), normal: V(0f, 0f, 1f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x - size, y - size, z + size), normal: V(0f, 0f, 1f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y - size, z + size), normal: V(0f, 0f, 1f), color, new UV(0f, 0f)));
            vertices.Add(new Vertex(position: V(x + size, y + size, z + size), normal: V(0f, 0f, 1f), color, new UV(0f, 0f)));
            break;

          default:
            throw new ArgumentOutOfRangeException(nameof(face), face, "An unrecognized face was requested.");
        }
      }

      public void Dispose() {
        lock (VertexCountSamples) {
          VertexCountSamples.Add(vertices.Count);
        }

        vertices.Dispose();
        indices.Dispose();
      }
    }
  }
}