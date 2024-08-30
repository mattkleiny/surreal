using Surreal.Colors;
using Surreal.IO;
using Surreal.Memory;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// Defines an Aseprite file and allows it to be parsed from disk.
/// <para/>
/// Aseprite stores its data in a proprietary binary format, which
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

    var header = AsepriteHeader.Load(reader);
    var file = new AsepriteFile(header);

    while (reader.Position < reader.Length)
    {
      var frame = AsepriteFrame.Load(reader, file.Header);

      file.Frames.Add(frame);
    }

    return file;
  }

  private AsepriteFile(AsepriteHeader header)
  {
    Header = header;
  }

  /// <summary>
  /// The header of the file.
  /// </summary>
  public AsepriteHeader Header { get; init; }

  /// <summary>
  /// The frames of the file.
  /// </summary>
  public List<AsepriteFrame> Frames { get; } = [];

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

      return new AsepriteHeader
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

    public required Size FileSize { get; init; }
    public required uint MagicNumber { get; init; }
    public required uint FrameCount { get; init; }
    public required uint Width { get; init; }
    public required uint Height { get; init; }
    public required ColorDepth ColorDepth { get; init; }
    public required uint Flags { get; init; }
    public required uint Speed { get; init; }
    public required byte TransparentIndex { get; init; }
    public required uint Colors { get; init; }
    public required byte PixelWidth { get; init; }
    public required byte PixelHeight { get; init; }
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

    public required Size Size { get; init; }
    public required uint MagicNumber { get; init; }
    public required uint ChunkCount { get; init; }
    public required TimeSpan Duration { get; init; }

    /// <summary>
    /// The header of the file.
    /// </summary>
    public AsepriteHeader Header => header;

    /// <summary>
    /// The chunks of the frame.
    /// </summary>
    public List<AsepriteChunk> Chunks { get; } = [];
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

  /// <summary>
  /// A chunk representing a single layer in the file.
  /// </summary>
  public sealed class AsepriteLayerChunk(AsepriteFrame frame) : AsepriteChunk(frame)
  {
    public new static AsepriteLayerChunk Load(FastBinaryReader reader, AsepriteFrame frame)
    {
      var flags = reader.ReadUInt16();
      var layerType = (LayerType)reader.ReadUInt16();
      var childLevel = reader.ReadUInt16();
      var defaultWidth = reader.ReadUInt16();
      var defaultHeight = reader.ReadUInt16();
      var blendMode = (LayerBlendMode)reader.ReadUInt16();
      var opacity = reader.ReadByte();

      reader.ReadBytes(3); // skip reserved bytes

      var name = reader.ReadString();

      return new AsepriteLayerChunk(frame)
      {
        Flags = flags,
        LayerType = layerType,
        ChildLevel = childLevel,
        DefaultWidth = defaultWidth,
        DefaultHeight = defaultHeight,
        BlendMode = blendMode,
        Opacity = opacity,
        Name = name,
      };
    }

    public required string Name { get; set; }
    public required uint Flags { get; set; }
    public required LayerType LayerType { get; set; }
    public required uint ChildLevel { get; set; }
    public required uint DefaultWidth { get; set; }
    public required uint DefaultHeight { get; set; }
    public required LayerBlendMode BlendMode { get; set; }
    public required byte Opacity { get; set; }
  }

  /// <summary>
  /// A chunk representing a single cel in the file.
  /// </summary>
  public sealed class AsepriteCelChunk(AsepriteFrame frame) : AsepriteChunk(frame)
  {
    public new static AsepriteCelChunk Load(FastBinaryReader reader, AsepriteFrame frame)
    {
      var layerIndex = reader.ReadUInt16();
      var offsetX = reader.ReadInt16();
      var offsetY = reader.ReadInt16();
      var opacity = reader.ReadByte();
      var celType = (CelType)reader.ReadUInt16();

      reader.ReadBytes(7); // skip reserved bytes

      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// A chunk representing a single cel in the file.
  /// </summary>
  public sealed class AsepriteCelExtraChunk(AsepriteFrame frame) : AsepriteChunk(frame)
  {
    public new static AsepriteCelExtraChunk Load(FastBinaryReader reader, AsepriteFrame frame)
    {
      var flags = reader.ReadUInt16();
      var preciseX = reader.ReadInt16();
      var preciseY = reader.ReadInt16();
      var preciseWidth = reader.ReadInt16();
      var preciseHeight = reader.ReadInt16();

      reader.ReadBytes(8); // skip reserved bytes

      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// A chunk containing tag data for the file.
  /// </summary>
  public sealed class AsepriteTagsChunk(AsepriteFrame frame) : AsepriteChunk(frame)
  {
    public new static AsepriteTagsChunk Load(FastBinaryReader reader, AsepriteFrame frame)
    {
      var tagCount = reader.ReadUInt16();

      reader.ReadBytes(8); // skip reserved bytes

      var tags = new List<TagEntry>();

      for (var i = 0; i < tagCount; i++)
      {
        tags.Add(TagEntry.Load(reader));
      }

      return new AsepriteTagsChunk(frame)
      {
        Tags = tags
      };
    }

    public required List<TagEntry> Tags { get; init; }

    /// <summary>
    /// A single tag in the file.
    /// </summary>
    public sealed class TagEntry
    {
      public static TagEntry Load(FastBinaryReader reader)
      {
        var fromFrame = reader.ReadUInt16();
        var toFrame = reader.ReadUInt16();
        var loopType = (LoopType)reader.ReadByte();
        var loopCount = reader.ReadUInt16();

        reader.ReadBytes(6); // skip reserved bytes

        var color = new Color32(
          reader.ReadByte(),
          reader.ReadByte(),
          reader.ReadByte(),
          reader.ReadByte()
        );

        var name = reader.ReadString();

        return new TagEntry
        {
          FromFrame = fromFrame,
          ToFrame = toFrame,
          LoopType = loopType,
          LoopCount = loopCount,
          Color = color,
          Name = name
        };
      }

      public required uint FromFrame { get; set; }
      public required uint ToFrame { get; set; }
      public required LoopType LoopType { get; set; }
      public required uint LoopCount { get; set; }
      public required Color32 Color { get; set; }
      public required string Name { get; set; }
    }
  }

  /// <summary>
  /// A chunk containing palette data for the file.
  /// </summary>
  public sealed class AsepritePaletteChunk(AsepriteFrame frame) : AsepriteChunk(frame)
  {
    public new static AsepritePaletteChunk Load(FastBinaryReader reader, AsepriteFrame frame)
    {
      var paletteSize = reader.ReadUInt32();
      var firstColorIndex = reader.ReadUInt32();
      var lastColorIndex = reader.ReadUInt32();

      reader.ReadBytes(8); // skip reserved bytes

      var colors = new List<Color32>((int)paletteSize);

      for (var i = 0; i < paletteSize; i++)
      {
        colors.Add(LoadColor(reader));
      }

      return new AsepritePaletteChunk(frame)
      {
        Colors = colors,
        FirstColorIndex = firstColorIndex,
        LastColorIndex = lastColorIndex,
      };
    }

    public required List<Color32> Colors { get; init; }
    public required uint FirstColorIndex { get; set; }
    public required uint LastColorIndex { get; set; }

    /// <summary>
    /// Loads a color from the palette.
    /// </summary>
    private static Color32 LoadColor(FastBinaryReader reader)
    {
      var flags = reader.ReadUInt16();

      var red = reader.ReadByte();
      var green = reader.ReadByte();
      var blue = reader.ReadByte();
      var alpha = reader.ReadByte();

      if ((flags & 1) != 0)
      {
        _ = reader.ReadString();
      }

      return new Color32(red, green, blue, alpha);
    }
  }

  public enum ColorDepth
  {
    Rgba = 32,
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

  public enum LayerType
  {
    Normal = 0,
    Group = 1,
  }

  public enum LayerBlendMode
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

  public enum LoopType
  {
    Forward = 0,
    Reverse = 1,
    PingPong = 2,
  }
}
