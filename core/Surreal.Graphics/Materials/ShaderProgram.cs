using Surreal.Collections;
using Surreal.Graphics.Textures;
using Surreal.IO;

namespace Surreal.Graphics.Materials;

/// <summary>
/// A low-level shader program on the GPU.
/// </summary>
public sealed class ShaderProgram(IGraphicsBackend backend) : GraphicsAsset
{
  /// <summary>
  /// Loads a <see cref="ShaderProgram"/> from the given <see cref="VirtualPath"/>.
  /// </summary>
  public static ShaderProgram Load(IGraphicsBackend backend, VirtualPath path)
  {
    using var stream = path.OpenInputStream();
    using var reader = new StreamReader(stream, Encoding.UTF8);

    var kernels = ParseCode(reader);
    var program = new ShaderProgram(backend);

    program.LinkKernels(kernels);

    return program;
  }

  /// <summary>
  /// Loads a <see cref="ShaderProgram"/> asynchronously from the given <see cref="VirtualPath"/>.
  /// </summary>
  public static async Task<ShaderProgram> LoadAsync(IGraphicsBackend backend, VirtualPath path, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();
    using var reader = new StreamReader(stream, Encoding.UTF8);

    var kernels = await ParseCodeAsync(reader, cancellationToken);
    var program = new ShaderProgram(backend);

    program.LinkKernels(kernels);

    return program;
  }

  /// <summary>
  /// Loads the built-in default canvas shader.
  /// </summary>
  public static ShaderProgram LoadDefaultCanvasShader(IGraphicsBackend backend)
  {
    return Load(backend, "resx://Surreal.Graphics/Assets/Embedded/shaders/canvas.glsl");
  }

  /// <summary>
  /// Loads the built-in default sprite shader.
  /// </summary>
  public static ShaderProgram LoadDefaultSpriteShader(IGraphicsBackend backend)
  {
    return Load(backend, "resx://Surreal.Graphics/Assets/Embedded/shaders/sprite.glsl");
  }

  /// <summary>
  /// Loads the built-in default wire shader.
  /// </summary>
  public static ShaderProgram LoadDefaultWireShader(IGraphicsBackend backend)
  {
    return Load(backend, "resx://Surreal.Graphics/Assets/Embedded/shaders/wire.glsl");
  }

  /// <summary>
  /// The <see cref="GraphicsHandle"/> for the shader itself.
  /// </summary>
  public GraphicsHandle Handle { get; private set; } = backend.CreateShader();

  /// <summary>
  /// Determines if the shader is currently active.
  /// </summary>
  public bool IsActive => backend.IsActiveShaderProgram(Handle);

  /// <summary>
  /// The <see cref="ShaderKernel"/>s that make up the program.
  /// </summary>
  public ReadOnlySlice<ShaderKernel> Kernels { get; private set; } = ReadOnlySlice<ShaderKernel>.Empty;

  /// <summary>
  /// Links the given <see cref="ShaderKernel"/>s to the program.
  /// </summary>
  internal void LinkKernels(ReadOnlySlice<ShaderKernel> kernels)
  {
    backend.LinkShader(Handle, kernels);

    Kernels = kernels;
  }

  /// <summary>
  /// Gets the uniform location for the given name.
  /// </summary>
  public int GetUniformLocation(string name)
  {
    return backend.GetShaderUniformLocation(Handle, name);
  }

  /// <summary>
  /// Attempts to get the uniform location for the given name.
  /// </summary>
  public bool TryGetUniformLocation(string name, out int location)
  {
    location = GetUniformLocation(name);

    return location != -1;
  }

  /// <summary>
  /// Sets the uniform value for the given name.
  /// </summary>
  public void SetUniform(string name, Variant value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      // @formatter:off
      switch (value.Type)
      {
        case VariantType.Bool: backend.SetShaderUniform(Handle, location, value.AsBool() ? 1 : 0); break;
        case VariantType.Byte: backend.SetShaderUniform(Handle, location, value.AsByte()); break;
        case VariantType.Short: backend.SetShaderUniform(Handle, location, value.AsShort()); break;
        case VariantType.Ushort: backend.SetShaderUniform(Handle, location, value.AsUshort()); break;
        case VariantType.Int: backend.SetShaderUniform(Handle, location, value.AsInt()); break;
        case VariantType.Uint: backend.SetShaderUniform(Handle, location, value.AsUint()); break;
        case VariantType.Long: backend.SetShaderUniform(Handle, location, value.AsLong()); break;
        case VariantType.Ulong: backend.SetShaderUniform(Handle, location, value.AsUlong()); break;
        case VariantType.Float: backend.SetShaderUniform(Handle, location, value.AsFloat()); break;
        case VariantType.Double: backend.SetShaderUniform(Handle, location, value.AsDouble()); break;
        case VariantType.Vector2: backend.SetShaderUniform(Handle, location, value.AsVector2()); break;
        case VariantType.Vector3: backend.SetShaderUniform(Handle, location, value.AsVector3()); break;
        case VariantType.Vector4: backend.SetShaderUniform(Handle, location, value.AsVector4()); break;
        case VariantType.Quaternion: backend.SetShaderUniform(Handle, location, value.AsQuaternion()); break;
        case VariantType.Color: backend.SetShaderUniform(Handle, location, value.AsColor()); break;
        case VariantType.Color32: backend.SetShaderUniform(Handle, location, value.AsColor32()); break;
        case VariantType.Object when value.AsObject() is Matrix3x2 matrix: backend.SetShaderUniform(Handle, location, matrix); break;
        case VariantType.Object when value.AsObject() is Matrix4x4 matrix: backend.SetShaderUniform(Handle, location, matrix); break;
        case VariantType.Object when value.AsObject() is Texture texture: backend.SetShaderSampler(Handle, location, texture.Handle, 0u); break;
        case VariantType.Object when value.AsObject() is TextureSampler sampler: backend.SetShaderSampler(Handle, location, sampler.Texture.Handle, sampler.SamplerSlot); break;

        default:
          throw new InvalidMaterialPropertyException($"The material property type for {name} is not supported.");
      }
      // @formatter:on
    }
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      backend.DeleteShader(Handle);
    }

    base.Dispose(managed);
  }


  /// <summary>
  /// Processes a GLSL program in the given <see cref="TextReader" /> and pre processes it with some useful features.
  /// </summary>
  private static ReadOnlySlice<ShaderKernel> ParseCode(TextReader reader)
  {
    var sharedCode = new StringBuilder();
    var shaderCode = new List<ShaderKernel>();

    foreach (var line in reader.ReadLines())
    {
      ParseShaderLine(line, shaderCode, sharedCode);
    }

    return shaderCode;
  }

  /// <summary>
  /// Processes a GLSL program in the given <see cref="TextReader" /> and pre processes it with some useful features.
  /// </summary>
  private static async Task<ReadOnlySlice<ShaderKernel>> ParseCodeAsync(TextReader reader, CancellationToken cancellationToken = default)
  {
    var sharedCode = new StringBuilder();
    var shaderCode = new List<ShaderKernel>();

    await foreach (var line in reader.ReadLinesAsync(cancellationToken))
    {
      ParseShaderLine(line, shaderCode, sharedCode);
    }

    return shaderCode;
  }

  /// <summary>
  /// Parses the given raw line of shader code.
  /// </summary>
  private static void ParseShaderLine(string line, List<ShaderKernel> kernels, StringBuilder sharedCode)
  {
    if (line.Trim().StartsWith("#shader_type"))
    {
      if (line.EndsWith("vertex"))
      {
        kernels.Add(new ShaderKernel(ShaderType.VertexShader, new StringBuilder(sharedCode.ToString())));
      }

      if (line.EndsWith("fragment"))
      {
        kernels.Add(new ShaderKernel(ShaderType.FragmentShader, new StringBuilder(sharedCode.ToString())));
      }
    }
    // append code to either globals or the last kernel
    else if (kernels.Count > 0)
    {
      kernels[^1].Code.AppendLine(line);
    }
    else
    {
      sharedCode.AppendLine(line);
    }
  }
}