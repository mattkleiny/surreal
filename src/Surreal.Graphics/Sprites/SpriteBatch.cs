using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Sprites
{
  /// <summary>An efficient batch of <see cref="Sprite"/>s for rendering to the GPU.</summary>
  public sealed class SpriteBatch : IDisposable
  {
    private const int MaximumSpriteCount = 8000;

    private static readonly MaterialProperty<Texture>   TextureView    = new("_Texture");
    private static readonly MaterialProperty<Matrix4x4> ProjectionView = new("_ProjectionView");

    private readonly IDisposableBuffer<Vertex> vertices;
    private readonly Mesh<Vertex>              mesh;

    private MaterialPass? materialPass;
    private Texture?      lastTexture;
    private int           vertexCount;

    public SpriteBatch(IGraphicsDevice device, int spriteCount)
    {
      Debug.Assert(spriteCount > 0, "spriteCount > 0");
      Debug.Assert(spriteCount < MaximumSpriteCount, "spriteCount < MaximumSpriteCount");

      Device             = device;
      MaximumVertexCount = spriteCount * 4;

      vertices = Buffers.AllocateNative<Vertex>(MaximumVertexCount);
      mesh     = new Mesh<Vertex>(device);

      CreateIndices(spriteCount * 6); // sprites are simple quads; we can create the indices up-front
    }

    public IGraphicsDevice Device             { get; }
    public Color           Color              { get; set; } = Color.White;
    public int             MaximumVertexCount { get; }

    public void Begin(MaterialPass materialPass, in Matrix4x4 projectionView)
    {
      this.materialPass = materialPass;

      materialPass.SetProperty(ProjectionView, projectionView);
    }

    public void Draw(Texture texture, float x, float y, Angle rotation, float width, float height)
    {
      DrawInternal(texture, x, y, width, height, rotation, 0, 0, texture.Width, texture.Height, Color);
    }

    private void DrawInternal(
        Texture texture,
        float x, float y,
        float width,
        float height,
        Angle rotation,
        float sourceX,
        float sourceY,
        float sourceWidth,
        float sourceHeight,
        Color color
    )
    {
      // if we're switching texture, we'll need to flush and start again
      if (texture != lastTexture)
      {
        Flush();
        lastTexture = texture;
      }
      // if we've exceeded the batch capacity, we'll need to flush and start again
      else if (vertexCount >= MaximumVertexCount)
      {
        Flush();
      }

      // calculate u/v extents
      var u  = sourceX / texture.Width;
      var v  = (sourceY + sourceHeight) / texture.Height;
      var u2 = (sourceX + sourceWidth) / texture.Width;
      var v2 = sourceY / texture.Height;

      // calculate shape extents
      var extentX = x + width;
      var extentY = y + height;

      // rotate coordinates about the z axis
      if (MathF.Abs(rotation.Radians) > float.Epsilon)
      {
        throw new NotImplementedException();
      }

      // add vertex data to our batch
      var vertices = this.vertices.Data[vertexCount..];

      ref var vertex0 = ref vertices[0];
      ref var vertex1 = ref vertices[1];
      ref var vertex2 = ref vertices[2];
      ref var vertex3 = ref vertices[3];

      vertex0.Position.X = x;
      vertex0.Position.Y = y;
      vertex0.Color      = color;
      vertex0.UV.X       = u;
      vertex0.UV.Y       = v;

      vertex1.Position.X = x;
      vertex1.Position.Y = extentY;
      vertex1.Color      = color;
      vertex1.UV.X       = u;
      vertex1.UV.Y       = v2;

      vertex2.Position.X = extentX;
      vertex2.Position.Y = extentY;
      vertex2.Color      = color;
      vertex2.UV.X       = u2;
      vertex2.UV.Y       = v2;

      vertex3.Position.X = extentX;
      vertex3.Position.Y = y;
      vertex3.Color      = color;
      vertex3.UV.X       = u2;
      vertex3.UV.Y       = v;

      vertexCount += 4;
    }

    public void Flush()
    {
      if (vertexCount == 0) return;
      if (materialPass == null) return;

      var spriteCount = vertexCount / 4;
      var indexCount  = spriteCount * 6;

      materialPass.SetProperty(TextureView, lastTexture!);

      mesh.Vertices.Write(vertices.Data[..indexCount]);
      mesh.DrawImmediate(materialPass, vertexCount, indexCount);

      vertexCount = 0;
    }

    private unsafe void CreateIndices(int indexCount)
    {
      Span<ushort> indices = stackalloc ushort[indexCount];

      for (ushort i = 0, j = 0; i < indexCount; i += 6, j += 4)
      {
        indices[i + 0] = j;
        indices[i + 1] = (ushort)(j + 1);
        indices[i + 2] = (ushort)(j + 2);
        indices[i + 3] = (ushort)(j + 2);
        indices[i + 4] = (ushort)(j + 3);
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
    private struct Vertex
    {
      [VertexDescriptor(
          Alias = "a_position",
          Count = 2,
          Type = VertexType.Float
      )]
      public Vector2 Position;

      [VertexDescriptor(
          Alias = "a_color",
          Count = 4,
          Type = VertexType.UnsignedByte,
          Normalized = true
      )]
      public Color Color;

      [VertexDescriptor(
          Alias = "a_texCoords",
          Count = 2,
          Type = VertexType.Float
      )]
      public Vector2 UV;

      public Vertex(Vector2 position, Color color, Vector2 uv)
      {
        Position = position;
        Color    = color;
        UV       = uv;
      }
    }
  }
}
