using System.Runtime.InteropServices;
using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Memory;

namespace Surreal.Graphics;

/// <summary>
/// A <see cref="RenderContext"/> for <see cref="Graphics.SpriteBatch"/>es.
/// </summary>
public sealed class SpriteContext(IGraphicsBackend backend) : RenderContext
{
  /// <summary>
  /// The <see cref="Graphics.SpriteBatch"/> used by this context.
  /// </summary>
  public SpriteBatch SpriteBatch { get; } = new(backend);

  /// <summary>
  /// The property to use for the projection view matrix.
  /// </summary>
  public MaterialProperty<Matrix4x4> ProjectionView { get; } = new("u_projectionView");

  /// <summary>
  /// The material used by this context.
  /// </summary>
  public Material Material { get; init; } = new(backend, ShaderProgram.LoadDefaultSpriteShader(backend))
  {
    BlendState = BlendState.OneMinusSourceAlpha
  };

  public override void OnBeginPass(in RenderFrame frame, IRenderViewport viewport)
  {
    base.OnBeginPass(in frame, viewport);

    Material.Properties.SetProperty(ProjectionView, viewport.ProjectionView);

    SpriteBatch.Begin(Material);
  }

  public override void OnEndPass(in RenderFrame frame, IRenderViewport viewport)
  {
    base.OnEndPass(in frame, viewport);

    SpriteBatch.Flush();
  }

  public override void Dispose()
  {
    SpriteBatch.Dispose();
    Material.Dispose();

    base.Dispose();
  }
}

/// <summary>
/// A batched mesh of sprites for rendering to the GPU.
/// </summary>
public sealed class SpriteBatch : IDisposable
{
  private const int DefaultSpriteCount = 200;
  private const int MaximumSpriteCount = int.MaxValue / 6;

  private readonly Mesh<Vertex2> _mesh;
  private readonly IDisposableBuffer<Vertex2> _vertices;

  private Texture? _lastTexture;
  private Material? _material;
  private int _vertexCount;

  public SpriteBatch(IGraphicsBackend backend, int spriteCount = DefaultSpriteCount)
  {
    Debug.Assert(spriteCount > 0, "spriteCount > 0");
    Debug.Assert(spriteCount <= MaximumSpriteCount, "spriteCount < MaximumSpriteCount");

    Backend = backend;

    _vertices = Buffers.AllocateNative<Vertex2>(spriteCount * 4);
    _mesh = new Mesh<Vertex2>(backend);

    CreateIndices(spriteCount * 6); // sprites are simple quads; we can create the indices up-front
  }

  /// <summary>
  /// Invoked when the batch is flushed.
  /// </summary>
  public event Action? Flushed;

  /// <summary>
  /// The underlying <see cref="IGraphicsBackend" />.
  /// </summary>
  public IGraphicsBackend Backend { get; }

  /// <summary>
  /// The property to use for the texture.
  /// </summary>
  public MaterialProperty<Texture> Texture { get; set; } = new("u_texture");

  public void Dispose()
  {
    _mesh.Dispose();
    _vertices.Dispose();
  }

  /// <summary>
  /// Begins a new batch of sprites with the given material.
  /// </summary>
  public void Begin(Material material)
  {
    _vertexCount = 0; // reset vertex pointer
    _material = material;
  }

  /// <summary>
  /// Draws a sprite at the given position.
  /// </summary>
  public void Draw(in TextureRegion region, Vector2 position)
  {
    Draw(region, position, region.Size);
  }

  /// <summary>
  /// Draws a sprite at the given position.
  /// </summary>
  public void Draw(in TextureRegion region, Vector2 position, Vector2 size)
  {
    Draw(region, position, size, Color32.White);
  }

  /// <summary>
  /// Draws a sprite at the given position.
  /// </summary>
  public void Draw(in TextureRegion region, Vector2 position, Vector2 size, Color32 color)
  {
    Draw(region, position, size, 0f, color);
  }

  /// <summary>
  /// Draws a sprite at the given position.
  /// </summary>
  [SkipLocalsInit]
  public void Draw(in TextureRegion region, Vector2 position, Vector2 size, float angle, Color32 color)
  {
    if (region.Texture == null)
    {
      return; // empty region? don't bother
    }

    if (region.Texture != _lastTexture)
    {
      // if we're switching texture, we'll need to flush and start again
      Flush();
      _lastTexture = region.Texture;
    }
    else if (_vertexCount + 4 >= _vertices.Span.Length)
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
    var output = new SpanList<Vertex2>(_vertices.Span[_vertexCount..]);

    output.AddUnchecked(new Vertex2(Vector2.Transform(new Vector2(-0.5f, -0.5f), transform), color, uv.BottomLeft));
    output.AddUnchecked(new Vertex2(Vector2.Transform(new Vector2(-0.5f, 0.5f), transform), color, uv.TopLeft));
    output.AddUnchecked(new Vertex2(Vector2.Transform(new Vector2(0.5f, 0.5f), transform), color, uv.TopRight));
    output.AddUnchecked(new Vertex2(Vector2.Transform(new Vector2(0.5f, -0.5f), transform), color, uv.BottomRight));

    _vertexCount += output.Count;
  }

  /// <summary>
  /// Flushes any pending sprites to the GPU.
  /// </summary>
  public void Flush()
  {
    if (_vertexCount == 0) return;

    if (_material != null)
    {
      if (_lastTexture != null)
      {
        _material.Properties.SetProperty(Texture, _lastTexture);
      }

      var spriteCount = _vertexCount / 4;
      var indexCount = spriteCount * 6;

      _mesh.Vertices.Write(_vertices.Span[.._vertexCount]);
      _mesh.Draw(_material, (uint)_vertexCount, (uint)indexCount);
    }

    _vertexCount = 0;
    Flushed?.Invoke();
  }

  /// <summary>
  /// Creates a default winding of indices for a quad.
  /// </summary>
  private unsafe void CreateIndices(int indexCount)
  {
    Span<uint> indices = stackalloc uint[indexCount];

    for (int i = 0, j = 0; i < indexCount; i += 6, j += 4)
    {
      indices[i + 0] = (uint)j;
      indices[i + 1] = (uint)(j + 1);
      indices[i + 2] = (uint)(j + 2);
      indices[i + 3] = (uint)(j + 2);
      indices[i + 4] = (uint)(j + 3);
      indices[i + 5] = (uint)j;
    }

    _mesh.Indices.Write(indices);
  }

  /// <summary>
  /// A common 2d vertex type for primitive shapes.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  private record struct Vertex2(Vector2 Position, Color32 Color, Vector2 UV)
  {
    [VertexDescriptor(2, VertexType.Float)]
    public Vector2 Position = Position;

    [VertexDescriptor(2, VertexType.Float)]
    public Vector2 UV = UV;

    [VertexDescriptor(4, VertexType.UnsignedByte, ShouldNormalize = true)]
    public Color32 Color = Color;
  }
}
