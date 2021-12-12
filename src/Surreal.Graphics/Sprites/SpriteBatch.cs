using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Sprites;

/// <summary>An efficient batch of sprites for rendering to the GPU.</summary>
public sealed class SpriteBatch : IDisposable
{
  private const int MaximumSpriteCount = 8000;

  private static readonly MaterialProperty<Texture>   TextureView    = new("_Texture");
  private static readonly MaterialProperty<Matrix4x4> ProjectionView = new("_ProjectionView");

  private readonly IDisposableBuffer<Vertex> vertices;
  private readonly Mesh<Vertex>              mesh;

  private Material? material;
  private Texture?  lastTexture;
  private int       vertexCount;

  public SpriteBatch(IGraphicsDevice device, int spriteCount)
  {
    Debug.Assert(spriteCount > 0, "spriteCount > 0");
    Debug.Assert(spriteCount < MaximumSpriteCount, "spriteCount < MaximumSpriteCount");

    Device = device;

    vertices = Buffers.AllocateNative<Vertex>(spriteCount * 4);
    mesh     = new Mesh<Vertex>(device);

    CreateIndices(spriteCount * 6); // sprites are simple quads; we can create the indices up-front
  }

  public IGraphicsDevice Device { get; }
  public Color           Color  { get; set; } = Color.White;

  public void Begin(Material material, in Matrix4x4 projectionView)
  {
    this.material = material;

    material.SetProperty(ProjectionView, projectionView);
  }

  public void Draw(in TextureRegion region, Vector2 position, Vector2 size, Angle rotation = default)
  {
    DrawInternal(region, position, size, rotation, region.Offset, region.Size, Color);
  }

  private void DrawInternal(
    in TextureRegion region,
    Vector2 position,
    Vector2 size,
    Angle rotation,
    Vector2I regionOffset,
    Vector2I regionSize,
    Color color
  )
  {
    // if we're switching texture, we'll need to flush and start again
    if (region.Texture != lastTexture)
    {
      Flush();
      lastTexture = region.Texture;
    }
    // if we've exceeded the batch capacity, we'll need to flush and start again
    else if (vertexCount >= this.vertices.Data.Length)
    {
      Flush();
    }

    // calculate u/v extents
    var u  = regionOffset.X / region.Width;
    var v  = (regionOffset.Y + regionSize.Y) / region.Height;
    var u2 = (regionOffset.X + regionSize.X) / region.Width;
    var v2 = regionOffset.Y / region.Height;

    // calculate shape extents
    var extentX = position.X + size.X;
    var extentY = position.Y + size.Y;

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

    vertex0.Position.X = position.X;
    vertex0.Position.Y = position.Y;
    vertex0.Color      = color;
    vertex0.UV.X       = u;
    vertex0.UV.Y       = v;

    vertex1.Position.X = position.X;
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
    vertex3.Position.Y = position.Y;
    vertex3.Color      = color;
    vertex3.UV.X       = u2;
    vertex3.UV.Y       = v;

    vertexCount += 4;
  }

  public void Flush()
  {
    if (vertexCount == 0) return;
    if (material == null) return;

    var spriteCount = vertexCount / 4;
    var indexCount  = spriteCount * 6;

    material.SetProperty(TextureView, lastTexture!);

    mesh.Vertices.Write(vertices.Data[..indexCount]);
    mesh.DrawImmediate(material, vertexCount, indexCount);

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
