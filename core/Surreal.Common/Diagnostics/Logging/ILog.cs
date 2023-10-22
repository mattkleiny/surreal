using Surreal.Collections;

namespace Surreal.Diagnostics.Logging;

/// <summary>
/// Different logging levels for <see cref="ILog" /> calls.
/// </summary>
public enum LogLevel
{
  Trace = 0,
  Debug = 1,
  Warn = 2,
  Error = 3,
  Fatal = 4
}

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

/// <summary>
/// A component capable of logging messages to some target.
/// </summary>
public interface ILog
{
  bool IsLevelEnabled(LogLevel level);

  void WriteMessage(LogLevel level, string message, Exception? exception = null);
  void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null);

  void Trace(string message)
  {
    if (IsLevelEnabled(LogLevel.Trace))
    {
      WriteMessage(LogLevel.Trace, message);
    }
  }

  void Trace(ref LogInterpolator handler)
  {
    if (IsLevelEnabled(LogLevel.Trace))
    {
      WriteMessage(LogLevel.Trace, ref handler);
    }
  }

  void Debug(string message)
  {
    if (IsLevelEnabled(LogLevel.Debug))
    {
      WriteMessage(LogLevel.Debug, message);
    }
  }

  void Debug(ref LogInterpolator handler)
  {
    if (IsLevelEnabled(LogLevel.Debug))
    {
      WriteMessage(LogLevel.Debug, ref handler);
    }
  }

  void Warn(string message)
  {
    if (IsLevelEnabled(LogLevel.Warn))
    {
      WriteMessage(LogLevel.Warn, message);
    }
  }

  void Warn(ref LogInterpolator handler)
  {
    if (IsLevelEnabled(LogLevel.Warn))
    {
      WriteMessage(LogLevel.Warn, ref handler);
    }
  }

  void Error(string message)
  {
    if (IsLevelEnabled(LogLevel.Error))
    {
      WriteMessage(LogLevel.Error, message);
    }
  }

  void Error(Exception exception, string message)
  {
    if (IsLevelEnabled(LogLevel.Error))
    {
      WriteMessage(LogLevel.Error, message, exception);
    }
  }

  void Error(ref LogInterpolator handler)
  {
    if (IsLevelEnabled(LogLevel.Error))
    {
      WriteMessage(LogLevel.Error, ref handler);
    }
  }

  void Error(Exception exception, ref LogInterpolator handler)
  {
    if (IsLevelEnabled(LogLevel.Error))
    {
      WriteMessage(LogLevel.Error, ref handler, exception);
    }
  }

  void Fatal(string message)
  {
    if (IsLevelEnabled(LogLevel.Fatal))
    {
      WriteMessage(LogLevel.Fatal, message);
    }
  }

  void Fatal(Exception exception, string message)
  {
    if (IsLevelEnabled(LogLevel.Fatal))
    {
      WriteMessage(LogLevel.Fatal, message, exception);
    }
  }

  void Fatal(ref LogInterpolator handler)
  {
    if (IsLevelEnabled(LogLevel.Fatal))
    {
      WriteMessage(LogLevel.Fatal, ref handler);
    }
  }

  void Fatal(Exception exception, ref LogInterpolator handler)
  {
    if (IsLevelEnabled(LogLevel.Fatal))
    {
      WriteMessage(LogLevel.Fatal, ref handler, exception);
    }
  }

  void Profile(string message, Action body)
  {
    var stopwatch = Stopwatch.StartNew();
    try
    {
      body();
    }
    finally
    {
      stopwatch.Stop();
      Trace($"{message}. Time taken: {stopwatch.Elapsed:g}");
    }
  }

  TResult Profile<TResult>(string message, Func<TResult> body)
  {
    var stopwatch = Stopwatch.StartNew();
    try
    {
      return body();
    }
    finally
    {
      stopwatch.Stop();
      Trace($"{message}. Time taken: {stopwatch.Elapsed:g}");
    }
  }
}
