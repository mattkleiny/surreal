using System.Runtime.CompilerServices;
using Surreal.Graphics.Images;

namespace Surreal.UI;

/// <summary>Represents content that can be rendered in the UI, containing text an optional tooltip and an optional icon.</summary>
public readonly record struct Content(string? Text, string? ToolTip, Image? Icon)
{
  public static implicit operator Content(string text) => new(text, ToolTip: null, Icon: null);
}

/// <summary>An interpolator for <see cref="Content"/>.</summary>
[InterpolatedStringHandler]
public readonly ref struct ContentInterpolator
{
  public ContentInterpolator(int literalLength, int formattedCount)
  {
    throw new NotImplementedException();
  }

  public void AppendLiteral(string value)
  {
    throw new NotImplementedException();
  }

  public void AppendFormatted<T>(T value)
  {
    throw new NotImplementedException();
  }

  public void AppendFormatted<T>(T value, string format)
    where T : IFormattable
  {
    throw new NotImplementedException();
  }

  public Content ToContent()
  {
    throw new NotImplementedException();
  }
}
