using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>An opaque handle to an graphics resource in the underling <see cref="IGraphicsServer"/> implementation.</summary>
public readonly record struct GraphicsId(uint Id)
{
  public GraphicsId(int id)
    : this((uint) id)
  {
  }

  public static implicit operator uint(GraphicsId id) => id.Id;
  public static implicit operator int(GraphicsId id)  => (int) id.Id;
}

/// <summary>An abstraction over the different types of graphics servers available.</summary>
public interface IGraphicsServer
{
  IBuffers   Buffers   { get; }
  ITextures  Textures  { get; }
  IShaders   Shaders   { get; }
  IMaterials Materials { get; }

  /// <summary>The buffer section of the API.</summary>
  public interface IBuffers
  {
    GraphicsId CreateBuffer();

    void UploadBufferData<T>(GraphicsId id, ReadOnlySpan<T> data) where T : unmanaged;
  }

  /// <summary>The texture section of the API.</summary>
  public interface ITextures
  {
    GraphicsId CreateTexture();

    void AllocateTexture(GraphicsId id, int width, int height, int depth, TextureFormat format);
    void UploadTextureData<T>(GraphicsId id, int width, int height, ReadOnlySpan<T> data, int mipLevel = 0) where T : unmanaged;
  }

  /// <summary>The shader section of the API.</summary>
  public interface IShaders
  {
    GraphicsId CreateShader();

    void CompileShader(GraphicsId id, ShaderDeclaration declaration);

    void SetShaderUniform(GraphicsId id, string name, int value);
    void SetShaderUniform(GraphicsId id, string name, float value);
    void SetShaderUniform(GraphicsId id, string name, Vector2I value);
    void SetShaderUniform(GraphicsId id, string name, Vector3I value);
    void SetShaderUniform(GraphicsId id, string name, Vector2 value);
    void SetShaderUniform(GraphicsId id, string name, Vector3 value);
    void SetShaderUniform(GraphicsId id, string name, Vector4 value);
    void SetShaderUniform(GraphicsId id, string name, Quaternion value);
    void SetShaderUniform(GraphicsId id, string name, in Matrix3x2 value);
    void SetShaderUniform(GraphicsId id, string name, in Matrix4x4 value);

    void DeleteShader(GraphicsId id);
  }

  /// <summary>The materials section of the API.</summary>
  public interface IMaterials
  {
  }
}
