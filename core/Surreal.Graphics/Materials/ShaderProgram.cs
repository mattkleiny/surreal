using Surreal.Collections.Slices;
using Surreal.Graphics.Textures;
using Surreal.IO;

namespace Surreal.Graphics.Materials;

/// <summary>
/// A low-level shader program on the GPU.
/// </summary>
public sealed class ShaderProgram(IGraphicsDevice device) : Disposable
{
  /// <summary>
  /// Loads the built-in default canvas shader.
  /// </summary>
  public static ShaderProgram LoadDefaultCanvasShader(IGraphicsDevice device)
    => Load(device, "resx://Surreal.Graphics/Assets/Embedded/shaders/shader-canvas.glsl");

  /// <summary>
  /// Loads the built-in default wire shader.
  /// </summary>
  public static ShaderProgram LoadDefaultWireShader(IGraphicsDevice device)
    => Load(device, "resx://Surreal.Graphics/Assets/Embedded/shaders/shader-wire.glsl");

  /// <summary>
  /// Loads the built-in default blit shader.
  /// </summary>
  public static ShaderProgram LoadDefaultBlitShader(IGraphicsDevice device)
    => Load(device, "resx://Surreal.Graphics/Assets/Embedded/shaders/shader-blit.glsl");

  /// <summary>
  /// Loads a <see cref="ShaderProgram"/> from the given <see cref="VirtualPath"/>.
  /// </summary>
  public static ShaderProgram Load(IGraphicsDevice device, VirtualPath path)
  {
    using var reader = path.OpenInputStreamReader();

    var kernels = ParseCode(reader);
    var program = new ShaderProgram(device);

    program.LinkKernels(kernels);

    return program;
  }

  /// <summary>
  /// Loads a <see cref="ShaderProgram"/> asynchronously from the given <see cref="VirtualPath"/>.
  /// </summary>
  public static async Task<ShaderProgram> LoadAsync(IGraphicsDevice device, VirtualPath path, CancellationToken cancellationToken = default)
  {
    using var reader = path.OpenInputStreamReader();

    var kernels = await ParseCodeAsync(reader, cancellationToken);
    var program = new ShaderProgram(device);

    program.LinkKernels(kernels);

    return program;
  }

  /// <summary>
  /// The <see cref="GraphicsHandle"/> for the shader itself.
  /// </summary>
  public GraphicsHandle Handle { get; } = device.CreateShader();

  /// <summary>
  /// Reloads the shader program from the given path.
  /// </summary>
  public async Task ReloadAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    using var reader = path.OpenInputStreamReader();

    var kernels = await ParseCodeAsync(reader, cancellationToken);

    LinkKernels(kernels);
  }

  /// <summary>
  /// Links the given <see cref="ShaderKernel"/>s to the program.
  /// </summary>
  private void LinkKernels(ReadOnlySlice<ShaderKernel> kernels)
  {
    device.LinkShader(Handle, kernels);
  }

  /// <summary>
  /// Gets the uniform location for the given name.
  /// </summary>
  public int GetUniformLocation(string name)
  {
    return device.GetShaderUniformLocation(Handle, name);
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
        case VariantType.Null: break; // no-op
        case VariantType.Bool: device.SetShaderUniform(Handle, location, value.AsBool() ? 1 : 0); break;
        case VariantType.Byte: device.SetShaderUniform(Handle, location, value.AsByte()); break;
        case VariantType.Short: device.SetShaderUniform(Handle, location, value.AsShort()); break;
        case VariantType.Ushort: device.SetShaderUniform(Handle, location, value.AsUshort()); break;
        case VariantType.Int: device.SetShaderUniform(Handle, location, value.AsInt()); break;
        case VariantType.Uint: device.SetShaderUniform(Handle, location, value.AsUint()); break;
        case VariantType.Long: device.SetShaderUniform(Handle, location, value.AsLong()); break;
        case VariantType.Ulong: device.SetShaderUniform(Handle, location, value.AsUlong()); break;
        case VariantType.Float: device.SetShaderUniform(Handle, location, value.AsFloat()); break;
        case VariantType.Double: device.SetShaderUniform(Handle, location, value.AsDouble()); break;
        case VariantType.Decimal: device.SetShaderUniform(Handle, location, (double) value.AsDecimal()); break;
        case VariantType.Point2: device.SetShaderUniform(Handle, location, value.AsPoint2()); break;
        case VariantType.Point3: device.SetShaderUniform(Handle, location, value.AsPoint3()); break;
        case VariantType.Point4: device.SetShaderUniform(Handle, location, value.AsPoint4()); break;
        case VariantType.Vector2: device.SetShaderUniform(Handle, location, value.AsVector2()); break;
        case VariantType.Vector3: device.SetShaderUniform(Handle, location, value.AsVector3()); break;
        case VariantType.Vector4: device.SetShaderUniform(Handle, location, value.AsVector4()); break;
        case VariantType.Quaternion: device.SetShaderUniform(Handle, location, value.AsQuaternion()); break;
        case VariantType.Color: device.SetShaderUniform(Handle, location, value.AsColor()); break;
        case VariantType.Color32: device.SetShaderUniform(Handle, location, value.AsColor32()); break;
        case VariantType.Object when value.AsObject() is Matrix3x2 matrix: device.SetShaderUniform(Handle, location, matrix); break;
        case VariantType.Object when value.AsObject() is Matrix4x4 matrix: device.SetShaderUniform(Handle, location, matrix); break;
        case VariantType.Object when value.AsObject() is Texture texture: device.SetShaderSampler(Handle, location, texture.Handle, 0u); break;
        case VariantType.Object when value.AsObject() is TextureSampler sampler: device.SetShaderSampler(Handle, location, sampler); break;

        default: throw new InvalidShaderPropertyException($"The material property type for {name} is not supported.");
      }
      // @formatter:on
    }
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      device.DeleteShader(Handle);
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
