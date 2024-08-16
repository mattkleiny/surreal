using Surreal.Memory;
using Image = Surreal.Graphics.Images.Image;
using Size = Surreal.Memory.Size;

namespace Surreal.Graphics.Textures;

/// <summary>
/// Formats for a <see cref="Texture" />.
/// </summary>
public enum TextureFormat
{
  R8,
  Rgb8,
  Rgba8,
  R,
  Rgb,
  Rgba
}

/// <summary>
/// Wrapping modes for a <see cref="Texture" />.
/// </summary>
public enum TextureWrapMode
{
  ClampToEdge,
  Repeat
}

/// <summary>
/// Filter modes for a <see cref="Texture" />.
/// </summary>
public enum TextureFilterMode
{
  Point,
  Linear
}

/// <summary>
/// A texture sampler configuration for a shader.
/// </summary>
public readonly record struct TextureSampler(GraphicsHandle Texture, uint SamplerSlot)
{
  /// <summary>
  /// The <see cref="TextureFilterMode"/> to apply to the sampler.
  /// </summary>
  public Optional<TextureFilterMode> FilterMode { get; init; }

  /// <summary>
  /// The <see cref="TextureWrapMode"/> to apply to the sampler.
  /// </summary>
  public Optional<TextureWrapMode> WrapMode { get; init; }
}

/// <summary>
/// A texture that can be uploaded to the GPU.
/// </summary>
[DebuggerDisplay("Texture {Width}x{Height} (Format {Format})")]
public sealed class Texture(IGraphicsDevice device, TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode) : Disposable
{
  private TextureWrapMode _wrapMode = wrapMode;
  private TextureFilterMode _filterMode = filterMode;

  /// <summary>
  /// The <see cref="GraphicsHandle"/> for the underlying texture.
  /// </summary>
  public GraphicsHandle Handle { get; } = device.CreateTexture(filterMode, wrapMode);

  /// <summary>
  /// The width of the texture, in pixels.
  /// </summary>
  public int Width { get; private set; }

  /// <summary>
  /// The height of the texture, in pixels.
  /// </summary>
  public int Height { get; private set; }

  /// <summary>
  /// The c<see cref="TextureFormat" /> of the texture.
  /// </summary>
  public TextureFormat Format { get; init; } = format;

  /// <summary>
  /// The <see cref="TextureFilterMode" /> of the texture.
  /// </summary>
  public TextureFilterMode FilterMode
  {
    get => _filterMode;
    set
    {
      _filterMode = value;
      device.SetTextureFilterMode(Handle, value);
    }
  }

  /// <summary>
  /// The <see cref="TextureWrapMode" /> of the texture.
  /// </summary>
  public TextureWrapMode WrapMode
  {
    get => _wrapMode;
    set
    {
      _wrapMode = value;
      device.SetTextureWrapMode(Handle, value);
    }
  }

  /// <summary>
  /// The size of the texture, in bytes.
  /// </summary>
  public Size Size { get; private set; }

  /// <summary>
  /// Converts the <see cref="Texture" /> to a <see cref="TextureRegion" />.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public TextureRegion ToRegion()
  {
    return new TextureRegion(this);
  }

  /// <summary>
  /// Reads the entire texture into a <see cref="Memory{T}" />.
  /// </summary>
  public GraphicsTask<Memory<T>> ReadPixelsAsync<T>()
    where T : unmanaged
  {
    return device.ReadTextureDataAsync<T>(Handle);
  }

  /// <summary>
  /// Reads a sub-region of the texture into a <see cref="Memory{T}" />.
  /// </summary>
  public GraphicsTask<Memory<T>> ReadPixelsAsync<T>(int offsetX, int offsetY, int width, int height)
    where T : unmanaged
  {
    return device.ReadTextureDataAsync<T>(Handle, offsetX, offsetY, (uint)width, (uint)height);
  }

  /// <summary>
  /// Writes the given <see cref="ReadOnlySpan{T}" /> to the texture.
  /// </summary>
  public GraphicsTask WritePixelsAsync<T>(int width, int height, ReadOnlySpan<T> span)
    where T : unmanaged
  {
    Width = width;
    Height = height;
    Size = span.CalculateSize();

    return device.WriteTextureDataAsync(Handle, (uint)width, (uint)height, span, Format);
  }

  /// <summary>
  /// Writes the given <see cref="Image" /> to the texture.
  /// </summary>
  public GraphicsTask WritePixelsAsync(Image image)
  {
    return WritePixelsAsync(image.Width, image.Height, image.Pixels.ToReadOnlySpan());
  }

  /// <summary>
  /// Writes the given <see cref="ReadOnlySpan{T}" /> to the texture at the given offset.
  /// </summary>
  public GraphicsTask WritePixelsAsync<T>(int offsetX, int offsetY, int width, int height, ReadOnlySpan<T> span)
    where T : unmanaged
  {
    return device.WriteTextureDataAsync(Handle, offsetX, offsetY, (uint)width, (uint)height, span);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      device.DeleteTexture(Handle);
    }

    base.Dispose(managed);
  }
}
