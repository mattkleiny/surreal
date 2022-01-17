using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Pipelines;

/// <summary>An opaque handle to a graphics resource in the underling graphics pipeline implementation.</summary>
public readonly record struct GraphicsId(uint Id)
{
  public GraphicsId(int id)
    : this((uint) id)
  {
  }

  public static implicit operator uint(GraphicsId id) => id.Id;
  public static implicit operator int(GraphicsId id)  => (int) id.Id;
}

/// <summary>An abstraction over the different types of graphics pipelines available.</summary>
public interface IGraphicsPipeline
{
  IBuffers   Buffers   { get; }
  ITextures  Textures  { get; }
  IShaders   Shaders   { get; }
  IMaterials Materials { get; }
  ILighting  Lighting  { get; }

  /// <summary>The buffer section of the API.</summary>
  public interface IBuffers
  {
    GraphicsId CreateBuffer();

    void UploadData<T>(GraphicsId id, ReadOnlySpan<T> data);
  }

  /// <summary>The texture section of the API.</summary>
  public interface ITextures
  {
    GraphicsId CreateTexture();

    void Allocate(GraphicsId id, int width, int height, int depth, TextureFormat format);
    void UploadTexels<T>(GraphicsId id, ReadOnlySpan<T> data, int mipLevel = 0);
  }

  /// <summary>The shader section of the API.</summary>
  public interface IShaders
  {
    GraphicsId CreateShader();

    void Compile(GraphicsId id, ShaderDeclaration declaration);
  }

  /// <summary>The materials section of the API.</summary>
  public interface IMaterials
  {
    GraphicsId CreateMaterial();

    void SetShader(GraphicsId materialId, GraphicsId shaderId);
  }

  /// <summary>The lighting section of the pipeline.</summary>
  public interface ILighting
  {
    GraphicsId CreateLight();

    void SetTransform(GraphicsId id, in Matrix4x4 transform);
    void SetShadowTransform(GraphicsId id, in Matrix4x4 transform);
  }
}
