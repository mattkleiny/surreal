﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Surreal.Collections;

namespace Surreal.Graphics.UI;

public static class CanvasTextExtensions
{
  public static void DrawText(this IImmediateModeCanvas layout, string text)
  {
    throw new NotImplementedException();
  }

  public static void DrawText(this IImmediateModeCanvas layout, ref TextInterpolator interpolator)
  {
    throw new NotImplementedException();
  }

  /// <summary>Allows pooled and deferred interpolated string construction in messages.</summary>
  [InterpolatedStringHandler]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public readonly ref struct TextInterpolator
  {
    private readonly StringBuilder builder;

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
    public TextInterpolator(int literalLength, int formattedCount)
    {
      builder = Pool<StringBuilder>.Shared.CreateOrRent();
    }

    public void AppendLiteral(string value)
    {
      builder.Append(value);
    }

    public void AppendFormatted<T>(T value)
    {
      builder.Append(value);
    }

    public void AppendFormatted<T>(T value, string format)
      where T : IFormattable
    {
      builder.Append(value.ToString(format, CultureInfo.InvariantCulture));
    }

    public string GetFormattedTextAndReturnToPool()
    {
      var result = builder.ToString();

      Pool<StringBuilder>.Shared.Return(builder);

      return result;
    }
  }
}