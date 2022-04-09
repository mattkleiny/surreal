namespace Surreal.Diagnostics.Logging;

/// <summary>Different logging levels for <see cref="ILog"/> calls.</summary>
public enum LogLevel
{
  Trace = 0,
  Debug = 1,
  Warn = 2,
  Error = 3,
  Fatal = 4,
}

/// <summary>A component capable of logging messages to some target.</summary>
public interface ILog
{
  bool IsLevelEnabled(LogLevel level);

  void WriteMessage(LogLevel level, string message);
  void WriteMessage(LogLevel level, ref LogInterpolator handler);

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

  void Error(ref LogInterpolator handler)
  {
    if (IsLevelEnabled(LogLevel.Error))
    {
      WriteMessage(LogLevel.Error, ref handler);
    }
  }

  void Fatal(string message)
  {
    if (IsLevelEnabled(LogLevel.Fatal))
    {
      WriteMessage(LogLevel.Fatal, message);
    }
  }

  void Fatal(ref LogInterpolator handler)
  {
    if (IsLevelEnabled(LogLevel.Fatal))
    {
      WriteMessage(LogLevel.Fatal, ref handler);
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
