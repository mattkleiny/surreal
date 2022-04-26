﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Sprites;

/// <summary>A batched mesh of sprites for rendering to the GPU.</summary>
public sealed class SpriteBatch : IDisposable
{
  private const int DefaultSpriteCount = 200;
  private const int MaximumSpriteCount = 8000;

  private readonly IDisposableBuffer<Vertex> vertices;
  private readonly Mesh<Vertex> mesh;

  private ShaderProgram? shader;
  private Texture? lastTexture;
  private int vertexCount;

  public SpriteBatch(IGraphicsServer server, int spriteCount = DefaultSpriteCount)
  {
    Debug.Assert(spriteCount > 0, "spriteCount > 0");
    Debug.Assert(spriteCount < MaximumSpriteCount, "spriteCount < MaximumSpriteCount");

    vertices = Buffers.AllocateNative<Vertex>(spriteCount * 4);
    mesh     = new Mesh<Vertex>(server);

    CreateIndices(spriteCount * 6); // sprites are simple quads; we can create the indices up-front
  }

  public void Begin(ShaderProgram shader)
    => this.shader = shader;

  public void Draw(in TextureRegion region, Vector2 position, Vector2 size, Color color)
    => DrawInternal(region, position, size, color);

  [SkipLocalsInit]
  private void DrawInternal(in TextureRegion region, Vector2 position, Vector2 size, Color color)
  {
    if (region.Texture != lastTexture)
    {
      // if we're switching texture, we'll need to flush and start again
      Flush();
      lastTexture = region.Texture;
    }
    else if (vertexCount >= vertices.Memory.Length)
    {
      // if we've exceeded the batch capacity, we'll need to flush and start again
      Flush();
    }

    // calculate u/v extents
    var uv = region.UV;

    // calculate shape extents
    var extentX = position.X + size.X;
    var extentY = position.Y + size.Y;

    // add vertex data to our batch
    var span = vertices.Memory.Span[vertexCount..];

    ref var vertex0 = ref span[0];
    vertex0.Position.X = position.X;
    vertex0.Position.Y = position.Y;
    vertex0.Color      = color;
    vertex0.UV.X       = uv.Left;
    vertex0.UV.Y       = uv.Bottom;

    ref var vertex1 = ref span[1];
    vertex1.Position.X = position.X;
    vertex1.Position.Y = extentY;
    vertex1.Color      = color;
    vertex1.UV.X       = uv.Left;
    vertex1.UV.Y       = uv.Top;

    ref var vertex2 = ref span[2];
    vertex2.Position.X = extentX;
    vertex2.Position.Y = extentY;
    vertex2.Color      = color;
    vertex2.UV.X       = uv.Right;
    vertex2.UV.Y       = uv.Top;

    ref var vertex3 = ref span[3];
    vertex3.Position.X = extentX;
    vertex3.Position.Y = position.Y;
    vertex3.Color      = color;
    vertex3.UV.X       = uv.Right;
    vertex3.UV.Y       = uv.Bottom;

    vertexCount += 4;
  }

  public void Flush()
  {
    if (vertexCount == 0) return;
    if (shader == null) return;

    var spriteCount = vertexCount / 4;
    var indexCount = spriteCount * 6;

    mesh.Vertices.Write(vertices.Memory.Span[..vertexCount]);
    mesh.Draw(shader, vertexCount, indexCount);

    vertexCount = 0;
  }

  private unsafe void CreateIndices(int indexCount)
  {
    Span<ushort> indices = stackalloc ushort[indexCount];

    for (ushort i = 0, j = 0; i < indexCount; i += 6, j += 4)
    {
      indices[i + 0] = j;
      indices[i + 1] = (ushort) (j + 1);
      indices[i + 2] = (ushort) (j + 2);
      indices[i + 3] = (ushort) (j + 2);
      indices[i + 4] = (ushort) (j + 3);
      indices[i + 5] = j;
    }

    mesh.Indices.Write(indices);
  }

  public void Dispose()
  {
    mesh.Dispose();
    vertices.Dispose();
  }

  [StructLayout(LayoutKind.Sequential)]
  private record struct Vertex(Vector2 Position, Color Color, Vector2 UV)
  {
    [VertexDescriptor(
      Count = 2,
      Type = VertexType.Float
    )]
    public Vector2 Position = Position;

    [VertexDescriptor(
      Count = 4,
      Type = VertexType.Float
    )]
    public Color Color = Color;

    [VertexDescriptor(
      Count = 2,
      Type = VertexType.Float
    )]
    public Vector2 UV = UV;
  }
}
