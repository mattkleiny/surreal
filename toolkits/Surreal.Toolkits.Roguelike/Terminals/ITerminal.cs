using Surreal.Graphics;
using Surreal.Graphics.Images;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Terminals;

/// <summary>A coloured character that can be painted.</summary>
public readonly record struct Glyph
{
  public char    Character       { get; init; }
  public Color32 ForegroundColor { get; init; }
  public Color32 BackgroundColor { get; init; }
}

/// <summary>A terminal for glyph-based rogue-like games.</summary>
public interface ITerminal
{
  void Paint(int x, int y, in Glyph glyph);
}

/// <summary>A <see cref="ITerminal"/> that paints to the <see cref="Console"/>.</summary>
public class ConsoleTerminal : ITerminal
{
  public void Paint(int x, int y, in Glyph glyph)
  {
    throw new NotImplementedException();
  }
}

/// <summary>A <see cref="ITerminal"/> that paints to a <see cref="Image"/>.</summary>
public class ImageTerminal : ITerminal, IDisposable
{
  public ImageTerminal(int width, int height)
  {
    Image = new(width, height);
  }

  public Image Image { get; }

  public void Paint(int x, int y, in Glyph glyph)
  {
    throw new NotImplementedException();
  }

  public virtual void Dispose()
  {
    Image.Dispose();
  }
}

/// <summary>A <see cref="ITerminal"/> that paints to a <see cref="Texture"/>.</summary>
public class TextureTerminal : ImageTerminal
{
  private readonly Texture texture;

  public TextureTerminal(int width, int height, IGraphicsDevice device)
    : base(width, height)
  {
    texture = device.CreateTexture(Image);
  }

  public override void Dispose()
  {
    texture.Dispose();

    base.Dispose();
  }
}
