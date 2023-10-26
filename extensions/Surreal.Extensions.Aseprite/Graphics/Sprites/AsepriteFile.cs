using Surreal.IO;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// Defines an Aseprite file and allows it to be parsed from disk.
/// <para/>
/// Aseprite stores it's data in a proprietary binary format, which
/// can be found here: https://github.com/aseprite/aseprite/blob/main/docs/ase-file-specs.md.
/// <para/>
/// Typically an Aseprite file is loaded as a <see cref="SpriteSheet"/>, though this
/// type is made public for more advanced scenarios.
/// </summary>
public sealed class AsepriteFile
{
  /// <summary>
  /// Loads an Aseprite file from the given <paramref name="stream"/>.
  /// </summary>
  public static AsepriteFile Load(Stream stream)
  {
    using var reader = new FastBinaryReader(stream);

    var result = new AsepriteFile();
    var header = new AsepriteHeader(result);

    while (reader.Position < reader.Length)
    {
      var chunk = AsepriteChunk.Load(reader, header);

      header.Chunks.Add(chunk);
    }

    return result;
  }

  private enum ColorDepth
  {
    RGBA = 32,
    Grayscale = 16,
    Indexed = 8,
  }

  private enum ChunkType
  {
    OldPaletteA = 0x0004,
    OldPaletteB = 0x0011,
    Layer = 0x2004,
    Cel = 0x2005,
    CelExtra = 0x2006,
    Mask = 0x2016,
    Path = 0x2017,
    Tags = 0x2018,
    Palette = 0x2019,
    UserData = 0x2020,
  }

  private enum LayerType
  {
    Normal = 0,
    Group = 1,
  }

  private enum LayerBlendMode
  {
    Normal = 0,
    Multiply = 1,
    Screen = 2,
    Overlay = 3,
    Darken = 4,
    Lighten = 5,
    ColorDodge = 6,
    ColorBurn = 7,
    HardLight = 8,
    SoftLight = 9,
    Difference = 10,
    Exclusion = 11,
    Hue = 12,
    Saturation = 13,
    Color = 14,
    Luminosity = 15,
    Addition = 16,
    Subtract = 17,
    Divide = 18,
  }

  private enum CelType
  {
    Raw = 0,
    Linked = 1,
    Compressed = 2,
  }

  private enum LoopType
  {
    Forward = 0,
    Reverse = 1,
    PingPong = 2,
  }

  /// <summary>
  /// The top-level header for the file.
  /// </summary>
  private sealed class AsepriteHeader(AsepriteFile file)
  {
    /// <summary>
    /// All of the chunks in the file.
    /// </summary>
    public List<AsepriteChunk> Chunks { get; } = new();
  }

  /// <summary>
  /// A single frame of the file.
  /// </summary>
  private sealed class AsepriteFrame(AsepriteHeader header);

  /// <summary>
  /// A chunk of binary data parsed from the file.
  /// </summary>
  private abstract class AsepriteChunk(AsepriteHeader header)
  {
    /// <summary>
    /// Loads the appropriate chunk from the given <paramref name="reader"/>.
    /// </summary>
    public static AsepriteChunk Load(FastBinaryReader reader, AsepriteHeader header)
    {
      throw new NotImplementedException();
    }
  }

  private sealed class AsepriteLayerChunk(AsepriteHeader header) : AsepriteChunk(header);
  private sealed class AsepriteCelChunk(AsepriteHeader header) : AsepriteChunk(header);
  private sealed class AsepriteCelExtraChunk(AsepriteHeader header) : AsepriteChunk(header);
  private sealed class AsepriteTagsChunk(AsepriteHeader header) : AsepriteChunk(header);
  private sealed class AsepritePaletteChunk(AsepriteHeader header) : AsepriteChunk(header);
}
