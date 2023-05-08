using Surreal.Collections;

namespace Surreal.Diagnostics.Logging;

/// <summary>
/// Allows pooled and deferred interpolated string construction in messages.
/// </summary>
[InterpolatedStringHandler]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public readonly ref struct LogInterpolator
{
  private readonly StringBuilder _builder;

  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
  public LogInterpolator(int literalLength, int formattedCount)
  {
    _builder = Pool<StringBuilder>.Shared.CreateOrRent();
  }

  public void AppendLiteral(string value)
  {
    _builder.Append(value);
  }

  public void AppendFormatted<T>(T value)
  {
    _builder.Append(value);
  }

  public void AppendFormatted<T>(T value, string format)
    where T : IFormattable
  {
    _builder.Append(value.ToString(format, CultureInfo.InvariantCulture));
  }

  public string GetFormattedTextAndReturnToPool()
  {
    var result = _builder.ToString();

    Pool<StringBuilder>.Shared.Return(_builder);

    return result;
  }
}
