using Surreal.Assets;
using Surreal.Graphics.Images.Sprites;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics.Images.FlipBooks;

/// <summary>A flip-book is a collection of named <see cref="SpriteAnimation"/>s.</summary>
public sealed class FlipBook
{
}

/// <summary>Parses Aseprite files into usable <see cref="FlipBook"/>s.</summary>
public sealed class AsepriteImporter : AssetImporter<FlipBook>
{
  public AsepriteImporter()
    : base(".ase", ".aseprite")
  {
  }

  public int                  FramesPerSecond  { get; set; } = 8;
  public int                  PixelsPerUnit    { get; set; } = 16;
  public TextureFilterMode    FilterMode       { get; set; } = TextureFilterMode.Point;
  public SpriteAnimationFlags AnimationFlags   { get; set; } = SpriteAnimationFlags.Looping;
  public Optional<Color32>    TransparencyMask { get; set; } = default;

  public override async ValueTask<FlipBook> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    await using var stream = await context.Path.OpenInputStreamAsync();

    var file = await AsepriteFile.LoadAsync(stream, progressToken.CancellationToken);

    throw new NotImplementedException();
  }

  /// <summary>Binary representation of an Aseprite file.</summary>
  private sealed record AsepriteFile
  {
    public static ValueTask<AsepriteFile> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
      using var reader = new BinaryReader(stream);

      throw new NotImplementedException();
    }

    public FileHeader Header { get; init; }

    public enum ColorDepth : ushort
    {
      Rgba      = 32,
      Grayscale = 16,
      Indexed   = 8
    }

    internal sealed class FileHeader
    {
      public uint       FileSize    { get; }
      public ushort     MagicNumber { get; }
      public ushort     Frames      { get; }
      public ushort     Width       { get; }
      public ushort     Height      { get; }
      public ColorDepth ColorDepth  { get; }
      public uint       Flags       { get; }
      public ushort     Speed       { get; }

      public byte TransparentIndex { get; }

      public ushort ColorCount  { get; }
      public byte   PixelWidth  { get; }
      public byte   PixelHeight { get; }
    }
  }
}

/// <summary>Parses PyxelEdit files into usable assets.</summary>
public sealed class PyxelImporter : AssetImporter<FlipBook>
{
  public override ValueTask<FlipBook> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    throw new NotImplementedException();
  }
}
