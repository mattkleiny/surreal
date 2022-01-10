﻿using System.Globalization;
using System.Runtime.CompilerServices;
using Surreal.Collections;

namespace Surreal.Diagnostics.Logging;

/// <summary>Allows pooled and deferred interpolated string construction in messages.</summary>
[InterpolatedStringHandler]
public readonly ref struct LogInterpolator
{
  private readonly StringBuilder builder;

  public LogInterpolator(int literalLength, int formattedCount)
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