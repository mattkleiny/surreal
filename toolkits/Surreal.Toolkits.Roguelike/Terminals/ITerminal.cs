using Surreal.Graphics;
using Surreal.Graphics.Images;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Terminals;

/// <summary>A coloured character that can be painted.</summary>
public sealed record Glyph
{
  public char    Character       { get; init; }
  public Color32 ForegroundColor { get; init; }
  public Color32 BackgroundColor { get; init; }
}

/// <summary>A terminal for glyph-based rogue-like games.</summary>
public interface ITerminal
{
  void Paint(Glyph glyph, int x, int y);
}

/// <summary>A <see cref="ITerminal"/> that paints to the <see cref="Console"/>.</summary>
public class ConsoleTerminal : ITerminal
{
  public void Paint(Glyph glyph, int x, int y)
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

  public void Paint(Glyph glyph, int x, int y)
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
