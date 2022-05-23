using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Surreal.Graphics.Materials;
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
  private const int MaximumSpriteCount = int.MaxValue / 6;

  private readonly IDisposableBuffer<Vertex2> vertices;
  private readonly Mesh<Vertex2> mesh;

  private Material? material;
  private Texture? lastTexture;
  private int vertexCount;

  public SpriteBatch(IGraphicsServer server, int spriteCount = DefaultSpriteCount)
  {
    Debug.Assert(spriteCount > 0, "spriteCount > 0");
    Debug.Assert(spriteCount <= MaximumSpriteCount, "spriteCount < MaximumSpriteCount");

    Server = server;

    vertices = Buffers.AllocateNative<Vertex2>(spriteCount * 4);
    mesh     = new Mesh<Vertex2>(server);

    CreateIndices(spriteCount * 6); // sprites are simple quads; we can create the indices up-front
  }

  public IGraphicsServer Server { get; }

  /// <summary>The <see cref="MaterialProperty{T}"/> to bind textures to.</summary>
  public MaterialProperty<Texture> TextureProperty { get; set; } = MaterialProperty.Texture;

  public void Begin(ShaderProgram shader)
    => Begin(new Material(shader));

  public void Begin(Material material)
  {
    vertexCount = 0; // reset vertex pointer

    this.material = material;
  }

  public void Draw(in TextureRegion region, Vector2 position)
    => Draw(region, position, region.Size);

  public void Draw(in TextureRegion region, Vector2 position, Vector2 size)
    => Draw(region, position, size, Color.White);

  public void Draw(in TextureRegion region, Vector2 position, Vector2 size, Color color)
    => Draw(region, position, size, 0f, color);

  [SkipLocalsInit]
  public void Draw(in TextureRegion region, Vector2 position, Vector2 size, float angle, Color color)
  {
    if (region.Texture == null) return; // empty region? don't bother
    if (region.Texture != lastTexture)
    {
      // if we're switching texture, we'll need to flush and start again
      Flush();
      lastTexture = region.Texture;
    }
    else if (vertexCount >= vertices.Span.Length)
    {
      // if we've exceeded the batch capacity, we'll need to flush and start again
      Flush();
    }

    // compute final transform from individual pieces
    var transform = MathF.Abs(angle) > float.Epsilon
      ? Matrix3x2.CreateScale(size) * Matrix3x2.CreateRotation(angle) * Matrix3x2.CreateTranslation(position)
      : Matrix3x2.CreateScale(size) * Matrix3x2.CreateTranslation(position);

    // compute UV texture bounds
    var uv = region.UV;

    // add results to sprite batch
    var output = new SpanList<Vertex2>(vertices.Span[vertexCount..]);

    output.AddUnchecked(new(Vector2.Transform(new(-0.5f, -0.5f), transform), color, uv.BottomLeft));
    output.AddUnchecked(new(Vector2.Transform(new(-0.5f, 0.5f), transform), color, uv.TopLeft));
    output.AddUnchecked(new(Vector2.Transform(new(0.5f, 0.5f), transform), color, uv.TopRight));
    output.AddUnchecked(new(Vector2.Transform(new(0.5f, -0.5f), transform), color, uv.BottomRight));

    vertexCount += output.Count;
  }

  public void Flush()
  {
    if (vertexCount == 0) return;
    if (material == null) return;

    var spriteCount = vertexCount / 4;
    var indexCount = spriteCount * 6;

    // bind the appropriate texture
    if (lastTexture != null)
    {
      material.Locals.SetProperty(TextureProperty, lastTexture);
    }

    mesh.Vertices.Write(vertices.Span[..vertexCount]);
    mesh.Draw(material, vertexCount, indexCount);

    vertexCount = 0;
  }

  private unsafe void CreateIndices(int indexCount)
  {
    Span<uint> indices = stackalloc uint[indexCount];

    for (int i = 0, j = 0; i < indexCount; i += 6, j += 4)
    {
      indices[i + 0] = (uint) j;
      indices[i + 1] = (uint) (j + 1);
      indices[i + 2] = (uint) (j + 2);
      indices[i + 3] = (uint) (j + 2);
      indices[i + 4] = (uint) (j + 3);
      indices[i + 5] = (uint) j;
    }

    mesh.Indices.Write(indices);
  }

  public void Dispose()
  {
    mesh.Dispose();
    vertices.Dispose();
  }

  /// <summary>A common 2d vertex type for primitive shapes.</summary>
  [StructLayout(LayoutKind.Sequential)]
  private record struct Vertex2(Vector2 Position, Color Color, Vector2 UV)
  {
    [VertexDescriptor(2, VertexType.Float)]
    public Vector2 Position = Position;

    [VertexDescriptor(4, VertexType.Float)]
    public Color Color = Color;

    [VertexDescriptor(2, VertexType.Float)]
    public Vector2 UV = UV;
  }
}
