using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Maths;
using Surreal.Memory;

namespace Surreal.Graphics.Canvases;

/// <summary>
/// A <see cref="RenderContext"/> for <see cref="CanvasBatch"/>es.
/// </summary>
public sealed class CanvasContext(IGraphicsBackend backend) : RenderContext
{
  /// <summary>
  /// The <see cref="CanvasBatch"/> used by this context.
  /// </summary>
  public CanvasBatch Batch { get; } = new(backend);

  /// <summary>
  /// The property to use for the projection view matrix.
  /// <para/>
  /// If not specified, the identity matrix will be used.
  /// </summary>
  public MaterialProperty<Matrix4x4>? TransformProperty { get; set; }

  /// <summary>
  /// The material used by this context.
  /// </summary>
  public Material Material { get; init; } = new(backend, ShaderProgram.LoadDefaultCanvasShader(backend))
  {
    BlendState = BlendState.OneMinusSourceAlpha
  };

  public override void OnBeginPass(in RenderFrame frame, IRenderViewport viewport)
  {
    base.OnBeginPass(in frame, viewport);

    if (TransformProperty != null)
    {
      Material.Properties.SetProperty(TransformProperty.Value, viewport.ProjectionView);
    }

    Batch.Material = Material;
    Batch.Reset();
  }

  public override void OnEndPass(in RenderFrame frame, IRenderViewport viewport)
  {
    base.OnEndPass(in frame, viewport);

    Batch.Flush();
  }

  public override void Dispose()
  {
    Batch.Dispose();
    Material.Dispose();

    base.Dispose();
  }
}

/// <summary>
/// A batched mesh of quads for rendering to the GPU.
/// </summary>
public sealed class CanvasBatch : IDisposable
{
  private const int DefaultQuadCount = 200;
  private const int MaximumQuadCount = int.MaxValue / 6;

  private readonly Mesh<Vertex2> _mesh;
  private readonly IDisposableBuffer<Vertex2> _vertices;

  private Texture? _lastTexture;
  private Material? _material;
  private int _vertexCount;

  public CanvasBatch(IGraphicsBackend backend, int quadCount = DefaultQuadCount)
  {
    Debug.Assert(quadCount > 0);
    Debug.Assert(quadCount <= MaximumQuadCount);

    Backend = backend;

    _vertices = Buffers.AllocateNative<Vertex2>(quadCount * 4);
    _mesh = new Mesh<Vertex2>(backend);

    CreateIndices(quadCount * 6);
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

  /// <summary>
  /// The material used by this batch.
  /// </summary>
  public Material? Material
  {
    get => _material;
    set
    {
      if (_material != value)
      {
        Flush();
        _material = value;
      }
    }
  }

  /// <summary>
  /// Draws a quad at the given position.
  /// </summary>
  public void DrawQuad(in TextureRegion region, Vector2 position)
    => DrawQuad(region, position, region.Size);

  /// <summary>
  /// Draws a quad at the given position.
  /// </summary>
  public void DrawQuad(in TextureRegion region, Vector2 position, Vector2 size)
    => DrawQuad(region, position, size, Color32.White);

  /// <summary>
  /// Draws a quad at the given position.
  /// </summary>
  public void DrawQuad(in TextureRegion region, Vector2 position, Vector2 size, Color32 color)
    => DrawQuad(region, position, size, 0f, color);

  /// <summary>
  /// Draws a quad at the given position.
  /// </summary>
  [SkipLocalsInit]
  public void DrawQuad(in TextureRegion region, Vector2 position, Vector2 size, float angle, Color32 color)
  {
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

    AddQuad(color, uv, stackalloc Vector2[]
    {
      Vector2.Transform(new Vector2(-0.5f, -0.5f), transform),
      Vector2.Transform(new Vector2(-0.5f, 0.5f), transform),
      Vector2.Transform(new Vector2(0.5f, 0.5f), transform),
      Vector2.Transform(new Vector2(0.5f, -0.5f), transform)
    });
  }

  /// <summary>
  /// Draws a quad at the given absolute position.
  /// </summary>
  [SkipLocalsInit]
  public void DrawQuadAbs(in TextureRegion region, Vector2 topLeft, Vector2 bottomRight, float angle, Color32 color)
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

    // compute UV texture bounds
    var uv = region.UV;

    AddQuad(color, uv, stackalloc Vector2[4]
    {
      new(topLeft.X, bottomRight.Y),
      new(topLeft.X, topLeft.Y),
      new(bottomRight.X, topLeft.Y),
      new(bottomRight.X, bottomRight.Y)
    });
  }

  /// <summary>
  /// Adds a quad to the batch.
  /// </summary>
  [SkipLocalsInit]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private void AddQuad(Color32 color, Rectangle uv, ReadOnlySpan<Vector2> vertices)
  {
    var storage = _vertices.Span[_vertexCount..];

    storage[0] = new Vertex2(vertices[0], color, uv.BottomLeft);
    storage[1] = new Vertex2(vertices[1], color, uv.TopLeft);
    storage[2] = new Vertex2(vertices[2], color, uv.TopRight);
    storage[3] = new Vertex2(vertices[3], color, uv.BottomRight);

    _vertexCount += 4;
  }

  /// <summary>
  /// Resets the batch, discarding any pending quads.
  /// </summary>
  public void Reset()
  {
    _vertexCount = 0; // reset vertex pointer
  }

  /// <summary>
  /// Flushes any pending quads to the GPU.
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

      var quadCount = _vertexCount / 4;
      var indexCount = quadCount * 6;

      _mesh.Vertices.Write(_vertices.Span[.._vertexCount]);
      _mesh.Draw(_material, (uint)_vertexCount, (uint)indexCount);
    }

    Reset();

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

  public void Dispose()
  {
    _mesh.Dispose();
    _vertices.Dispose();
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
