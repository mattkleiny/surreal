using Surreal.IO;
using Surreal.Memory;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// Defines an Aseprite file and allows it to be parsed from disk.
/// <para/>
/// Aseprite stores it's data in a proprietary binary format, which
/// can be found here: https://github.com/aseprite/aseprite/blob/main/docs/ase-file-specs.md.
/// </summary>
public sealed class AsepriteFile
{
  /// <summary>
  /// Loads an Aseprite file from the given <paramref name="stream"/>.
  /// </summary>
  public static AsepriteFile Load(Stream stream)
  {
    using var reader = new FastBinaryReader(stream);

    var file = new AsepriteFile
    {
      Header = AsepriteHeader.Load(reader)
    };

    while (reader.Position < reader.Length)
    {
      var frame = AsepriteFrame.Load(reader, file.Header);

      file.Frames.Add(frame);
    }

    return file;
  }

  /// <summary>
  /// The header of the file.
  /// </summary>
  public required AsepriteHeader Header { get; init; }

  /// <summary>
  /// The frames of the file.
  /// </summary>
  public List<AsepriteFrame> Frames { get; init; } = new();

  public enum ColorDepth
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
  public sealed class AsepriteHeader
  {
    public static AsepriteHeader Load(FastBinaryReader reader)
    {
      var fileSize = reader.ReadUInt32();
      var magicNumber = reader.ReadUInt16();
      var frameCount = reader.ReadUInt16();
      var width = reader.ReadUInt16();
      var height = reader.ReadUInt16();
      var colorDepth = (ColorDepth)reader.ReadUInt16();
      var flags = reader.ReadUInt32();
      var speed = reader.ReadUInt16();

      reader.ReadBytes(8); // skip reserved bytes

      var transparentIndex = reader.ReadByte();

      reader.ReadBytes(3); // skip reserved bytes

      var colors = reader.ReadUInt16();
      var pixelWidth = reader.ReadByte();
      var pixelHeight = reader.ReadByte();

      reader.ReadBytes(92); // skip reserved bytes

      return new AsepriteHeader()
      {
        FileSize = Size.FromBytes((int)fileSize),
        MagicNumber = magicNumber,
        FrameCount = frameCount,
        Width = width,
        Height = height,
        ColorDepth = colorDepth,
        Flags = flags,
        Speed = speed,
        TransparentIndex = transparentIndex,
        Colors = colors,
        PixelWidth = pixelWidth,
        PixelHeight = pixelHeight,
      };
    }

    public Size FileSize { get; init; }
    public uint MagicNumber { get; init; }
    public uint FrameCount { get; init; }
    public uint Width { get; init; }
    public uint Height { get; init; }
    public ColorDepth ColorDepth { get; init; }
    public uint Flags { get; init; }
    public uint Speed { get; init; }
    public byte TransparentIndex { get; init; }
    public uint Colors { get; init; }
    public byte PixelWidth { get; init; }
    public byte PixelHeight { get; init; }
  }

  /// <summary>
  /// A single frame of the file.
  /// </summary>
  public sealed class AsepriteFrame(AsepriteHeader header)
  {
    public static AsepriteFrame Load(FastBinaryReader reader, AsepriteHeader header)
    {
      var length = reader.ReadUInt32();
      var magicNumber = reader.ReadUInt16();
      var chunkCount = reader.ReadUInt16();
      var durationMs = reader.ReadUInt16();

      reader.ReadBytes(2); // skip reserved bytes

      var newChunkCount = reader.ReadUInt32();
      if (newChunkCount != 0)
        chunkCount = newChunkCount;

      var frame = new AsepriteFrame(header)
      {
        Size = Size.FromBytes((int)length),
        MagicNumber = magicNumber,
        ChunkCount = chunkCount,
        Duration = TimeSpan.FromMilliseconds(durationMs),
      };

      for (var i = 0; i < frame.ChunkCount; i++)
      {
        var chunk = AsepriteChunk.Load(reader, frame);
        if (chunk != null)
        {
          frame.Chunks.Add(chunk);
        }
      }

      return frame;
    }

    public Size Size { get; set; }
    public uint MagicNumber { get; set; }
    public uint ChunkCount { get; set; }
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// The chunks of the frame.
    /// </summary>
    public List<AsepriteChunk> Chunks { get; } = new();
  }

  /// <summary>
  /// A chunk of binary data parsed from the file.
  /// </summary>
  public abstract class AsepriteChunk(AsepriteFrame frame)
  {
    /// <summary>
    /// Loads the appropriate chunk from the given <paramref name="reader"/>.
    /// </summary>
    public static AsepriteChunk? Load(FastBinaryReader reader, AsepriteFrame frame)
    {
      const int headerSize = 6; // size of the header of a chunk

      var length = reader.ReadUInt32();
      var type = (ChunkType)reader.ReadUInt16();

      switch (type)
      {
        // case ChunkType.Layer:
        // case ChunkType.Cel:
        // case ChunkType.CelExtra:
        // case ChunkType.Tags:
        // case ChunkType.Palette:
        //   throw new NotImplementedException();

        // skip the chunk
        default:
          reader.Stream.Seek(length - headerSize, SeekOrigin.Current);
          return null;
      }
    }
  }

  private sealed class AsepriteLayerChunk(AsepriteFrame header) : AsepriteChunk(header);
  private sealed class AsepriteCelChunk(AsepriteFrame header) : AsepriteChunk(header);
  private sealed class AsepriteCelExtraChunk(AsepriteFrame header) : AsepriteChunk(header);
  private sealed class AsepriteTagsChunk(AsepriteFrame header) : AsepriteChunk(header);
  private sealed class AsepritePaletteChunk(AsepriteFrame header) : AsepriteChunk(header);
}
